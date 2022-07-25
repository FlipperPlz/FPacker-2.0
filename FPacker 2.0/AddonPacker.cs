using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RAP.Models.Values;
using FPacker.Models.AddonFiles;
using FPacker.P3D.Models;
using FPacker.P3D.Models.MLOD;
using FPacker.P3D.Models.ODOL;
using FPacker.PBO;
using FPacker.PBO.Enums;
using FPacker.PBO.Models;
using NLog;
using NLog.Config;
using NLog.Fluent;
using static System.String;

namespace FPacker;

public struct AddonPrefix {
    public string SystemPath { get; set; } //This is the prefix root established by where the config is
    public string PBOPath; //This is the pbo path to the directory containing that prefix
    public string PrefixName;
}



public struct AddonFile {
    public string PBOPath { get; set; }
    public string PBOReferencePath { get; set; }
    public string SystemPath { get; set; }
    public string PBOObfuscatedPath { get; set; }
    public string PBOObfuscatedRefPath { get; set; }

    public EntryDataType Type { get; set; }
}

public class AddonPacker {
    public static readonly Logger Logger = LogManager.GetLogger("FPacker");
    
    private readonly string _srcFolder;
    private readonly List<AddonPrefix> _prefixes = new();
    private readonly List<AddonFile> _files = new();
    
    private readonly Dictionary<string, string> _obfuscatedPaths = new();
    
    private readonly List<string> foundPaths = new();
    private readonly List<string> _foundRvMatPaths = new();
    private readonly List<string> _foundModelPaths = new();
    private readonly List<string> _foundSamplePaths = new();

    private readonly List<ConfigFile> _configs = new();

    private readonly List<EnforceFile> _scripts = new();
    private readonly List<RvMatFile> _materials = new();
    private readonly List<AddonFile> _audioFiles = new();
    private readonly List<P3DFile> _models = new();
    


    public AddonPacker(string sourceFolder, string outPath) {
        InitializeLogger();
        Logger.Info("Parsing configs in {srcFolder}.", sourceFolder);

        foreach (var file in new DirectoryInfo(sourceFolder).GetFiles("config.cpp", SearchOption.AllDirectories)) {
            Logger.Info("Parsing config: {fileName}", file.FullName);
            var pboPath = PBOUtilities.GetRelativePath(sourceFolder, file.FullName);
            var cfg = new ConfigFile(pboPath, file.FullName, file.FullName);
            _configs.Add(cfg);
            _prefixes.AddRange(cfg.PrefixObjs);
        }
        foreach (var config in _configs.ToList()) {
            config.MissionScriptPaths.ForEach(p => LoadScripts(p, 5));
            config.WorldScriptPaths.ForEach(p => LoadScripts(p, 4));
            config.EngineScriptPaths.ForEach(p => LoadScripts(p, 3));
            config.GameLibScriptPaths.ForEach(p => LoadScripts(p, 2));
            config.GameScriptPaths.ForEach(p => LoadScripts(p, 1));
        }
        
        var entries = new List<PBOEntry>();

        _configs.Add(new ConfigFile() {
            PBOPath = PBOUtilities.RandomString(44, "", true),
            ObjectBase = CreateModConfig()
        });
        
        foreach (var configFile in _configs) {
            configFile.ObfuscatedPaths = _obfuscatedPaths;
            entries.Add(new PBOEntry( configFile.PBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.BinarizedPoseidonConfig, configFile.ObjectBase.BinarizedData(), configFile.SystemPath));
        }

        foreach (var script in _scripts) {
            entries.Add(new PBOEntry( script.PBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.BinarizedPoseidonConfig, Encoding.UTF8.GetBytes(script.ObjectBase.ToEnforceFormat()), script.SystemPath));

        }

        new PBOStream(File.OpenWrite(outPath)).WritePBO(entries);
        
        
        
        

        Logger.Log(LogLevel.Info, JsonSerializer.Serialize(_prefixes.Select(static p => p.PrefixName)));
    }

    private void LoadScripts(string referenceScriptPath, int module) {
        var systemPath = ConvertPBORefPath2SystemPath(referenceScriptPath);
        if (File.Exists(systemPath)) {
            if(!_obfuscatedPaths.ContainsKey(referenceScriptPath)) _obfuscatedPaths.Add(referenceScriptPath, string.Empty);
            Logger.Info($"Identified script path: {referenceScriptPath} -> {systemPath}");
            var prefixRefPath = PBOUtilities.RandomString(500, "       \\\\\\", false, false);
            var prefixName = PBOUtilities.RandomString(13, string.Empty, false, false);
            var newFileName = PBOUtilities.RandomString(25, string.Empty, false, false);
            
            CreatePrefix(new AddonPrefix() {
                PBOPath = prefixRefPath,
                PrefixName = prefixName
            });

            if (_scripts.Select(static s => s.SystemPath).Contains(systemPath)) {
                foreach (var enforceFile in _scripts.Where(s => s.SystemPath == systemPath)) {
                    enforceFile.Modules.Add(module);
                }
                return;
            }
                
            _scripts.Add(new EnforceFile(prefixRefPath + Path.DirectorySeparatorChar + newFileName,
                prefixName + Path.DirectorySeparatorChar + newFileName, systemPath) {
                Modules = new List<int>() {module}
            });
            

        } else if (Directory.Exists(systemPath)) {
            if(!_obfuscatedPaths.ContainsKey(referenceScriptPath)) _obfuscatedPaths.Add(referenceScriptPath, string.Empty);
            Logger.Info($"Identified system folder path for {referenceScriptPath}, enumerating files.");

            foreach (var scriptPath in new DirectoryInfo(systemPath).EnumerateFiles("*.c", SearchOption.AllDirectories)) {
                var prefixRefPath = PBOUtilities.RandomString(500, "       \\\\\\", false, false);
                var prefixName = PBOUtilities.RandomString(13, string.Empty, false, false);
                var newFileName = PBOUtilities.RandomString(25, string.Empty, false, false);

                CreatePrefix(new AddonPrefix() {
                    PBOPath = prefixRefPath,
                    PrefixName = prefixName
                });

                if (_scripts.Select(static s => s.SystemPath).Contains(scriptPath.FullName)) {
                    foreach (var enforceFile in _scripts.Where(s => s.SystemPath == scriptPath.FullName)) {
                        enforceFile.Modules.Add(module);
                    }
                    continue;
                }
                
                _scripts.Add(new EnforceFile(prefixRefPath + Path.DirectorySeparatorChar + newFileName,
                    prefixName + Path.DirectorySeparatorChar + newFileName, scriptPath.FullName) {
                    Modules = new List<int>() {module}
                });

            }
        }
    }

    private RapFile CreateModConfig() {
        var enginePaths = new List<string>();
        var gameLibPaths = new List<string>();
        var gamePaths = new List<string>();
        var worldPaths = new List<string>();
        var missionPaths = new List<string>();

        foreach (var script in _scripts) {
            foreach (var module in script.Modules) {
                switch (module) {
                    case 1:
                        enginePaths.Add(script.PBOReferencePath);
                        continue;
                    case 2:
                        gameLibPaths.Add(script.PBOReferencePath);
                        continue;
                    case 3:
                        gamePaths.Add(script.PBOReferencePath);
                        continue;
                    case 4:
                        worldPaths.Add(script.PBOReferencePath);
                        continue;
                    case 5:
                        missionPaths.Add(script.PBOReferencePath);
                        continue;
                }
            }
        }

        return new RapFile() {
            GlobalClasses = new List<RapClass>() {
                new RapClass() {
                    ClassName = "CfgMods",
                    ChildClasses = new List<RapClass> {
                        new RapClass() {
                            ClassName = _prefixes.First().PrefixName,
                            ChildClasses = new List<RapClass>() {
                                new RapClass() {
                                    ClassName = "defs",
                                    ChildClasses = new List<RapClass>() {
                                        new RapClass() {
                                            ClassName = "engineScriptModule",
                                            VariableStatements = new List<RapVariableStatement>() {
                                                new RapVariableStatement() {
                                                    VariableName = "value", VariableValue = new RapString(string.Empty)
                                                },
                                                new RapVariableStatement() {
                                                    VariableName = "files", VariableValue = new RapArray(enginePaths)
                                                }
                                            }
                                        },
                                        new RapClass() {
                                            ClassName = "gameLibScriptModule",
                                            VariableStatements = new List<RapVariableStatement>() {
                                                new RapVariableStatement() {
                                                    VariableName = "value", VariableValue = new RapString(string.Empty)
                                                },
                                                new RapVariableStatement() {
                                                    VariableName = "files", VariableValue = new RapArray(gameLibPaths)
                                                }
                                            }
                                        },
                                        new RapClass() {
                                            ClassName = "gameScriptModule",
                                            VariableStatements = new List<RapVariableStatement>() {
                                                new RapVariableStatement() {
                                                    VariableName = "value", VariableValue = new RapString(string.Empty)
                                                },
                                                new RapVariableStatement() {
                                                    VariableName = "files", VariableValue = new RapArray(gamePaths)
                                                }
                                            }
                                        },
                                        new RapClass() {
                                            ClassName = "worldScriptModule",
                                            VariableStatements = new List<RapVariableStatement>() {
                                                new RapVariableStatement() {
                                                    VariableName = "value", VariableValue = new RapString(string.Empty)
                                                },
                                                new RapVariableStatement() {
                                                    VariableName = "files", VariableValue = new RapArray(worldPaths)
                                                }
                                            }
                                        },
                                        new RapClass() {
                                            ClassName = "missionScriptModule",
                                            VariableStatements = new List<RapVariableStatement>() {
                                                new RapVariableStatement() {
                                                    VariableName = "value", VariableValue = new RapString(string.Empty)
                                                },
                                                new RapVariableStatement() {
                                                    VariableName = "files", VariableValue = new RapArray(missionPaths)
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

        private void InitializeLogger() {
        NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"..\..\..\NLog.config",true);
    }

    public void CreatePrefix(AddonPrefix prefix) {
        var prefixCfg = new RapFile {
            GlobalClasses = new List<RapClass> {
                new() {
                    ClassName = "CfgPatches",
                    ChildClasses = new List<RapClass>() {
                        new() {
                            ClassName = prefix.PrefixName,
                            VariableStatements = new List<RapVariableStatement>() {
                                new() {VariableName = "requiredAddons", VariableValue = new RapArray(new[] {"DZ_Data"})}
                            }
                        }
                    }
                }
            }
        };
        var r = new ConfigFile() {
            PBOPath = prefix.PBOPath,
            PBOReferencePath = prefix.PrefixName + Path.DirectorySeparatorChar,
            ObjectBase = prefixCfg
        };
        _configs.Add(r);
    }

    private string? ConvertPBORefPath2SystemPath(string pboRefPath) {
        if (pboRefPath.StartsWith("#", StringComparison.Ordinal)) return null;
        if (pboRefPath.ToLower().StartsWith("dz", StringComparison.Ordinal)) return null;

        var usedPrefix = pboRefPath.Split(new[] {Path.DirectorySeparatorChar}, 2);
        foreach (var prefix in _prefixes.Where(prefix => string.Equals(prefix.PrefixName, usedPrefix[0], StringComparison.CurrentCultureIgnoreCase))) {
            return (prefix.SystemPath + usedPrefix[1]).TrimStart(Path.DirectorySeparatorChar);
        }

        if(usedPrefix[0].ToLower() == "dz" || pboRefPath.ToLower().TrimStart(Path.DirectorySeparatorChar).StartsWith("dz")) Logger.Debug("Found reference to DayZ path: {dzp}", pboRefPath);
        else Logger.Log(LogLevel.Warn, "File {pboRefPath} was found as a reference with the prefix of {usedPrefix}, but a system path could not be identified," +
                                       " if this is a mistake, it may lead to the file not getting packed." +
                                       " If this file is infact in another pbo or a basegame asset, you may ignore this error.", pboRefPath, usedPrefix[0]);
        return null;
    }

    private AddonPrefix DeterminePrefixFromPath(string pboRefPath) {
        var usedPrefix = pboRefPath.Split(new[] {Path.DirectorySeparatorChar}, 2);
        return _prefixes.FirstOrDefault(p => string.Equals(p.PrefixName, usedPrefix[0], StringComparison.CurrentCultureIgnoreCase));
    }
    
    private string ConvertPBORefPath2PBOPath(string pboRefPath) {
        var usedPrefix = pboRefPath.Split(new[] {Path.DirectorySeparatorChar}, 2);
        return _prefixes.FirstOrDefault(p => string.Equals(p.PrefixName, usedPrefix[0], StringComparison.CurrentCultureIgnoreCase)).PBOPath + usedPrefix[1];
    }

    private static string? SanitizePBOPath(string path) {
        var output = path.Replace("/", "\0").Replace("\\\\", "\0").Replace("\\", "\0").Replace("\0", Path.DirectorySeparatorChar.ToString());

        if(output.StartsWith(Path.DirectorySeparatorChar) && !output.ToLower().StartsWith(Path.DirectorySeparatorChar + "dz")) {
            Logger.Error(
            "Found a path starting with a directory separator.");}
        return output.ToLower();
    }
    
    private static bool ScanForPath(string pboPath, string addonFolder) {
        pboPath = pboPath.Replace("\\\\", "\0")
            .Replace("\\", "\0").
            Replace("/", "\0").Replace("\0", Path.DirectorySeparatorChar.ToString());
        Console.Out.WriteLineAsync($"PBOPath = {pboPath}");
        Console.Out.WriteLineAsync(Path.Combine(addonFolder, pboPath.Split(new []{Path.DirectorySeparatorChar}, 2).Last()));

        return true;
    }


    private static EntryDataType DetermineFileDataType(string file) {
        ArgumentNullException.ThrowIfNull(file);
        var fileExtension = file.Split(".").Last();

        // Warning ignored to show that there is a variable "fileType" being updated in the next regions
        // ReSharper disable once JoinDeclarationAndInitializer
        EntryDataType fileType;
        
        #region First, determine implied data type via file extension.
        fileType = fileExtension switch {
            ".c" => EntryDataType.EnforceScript,
            ".p3d" => EntryDataType.Model,
            ".paa" => EntryDataType.Texture,
            ".rvmat" => EntryDataType.RVMAT,
            ".html" => EntryDataType.HtmlGui,
            ".layout" => EntryDataType.GuiLayout,
            ".png" => EntryDataType.Asset,
            ".ogg" => EntryDataType.Audio,
            ".wav" => EntryDataType.Audio,
            ".wss" => EntryDataType.Audio,
            ".csv" => EntryDataType.Unknown,
            ".cpp" => EntryDataType.Unknown, //Data type will be determined by filename
            ".bin" => EntryDataType.UnknownBinarized, //Data type will be determined by filename
            _ => EntryDataType.Unknown
        };
        #endregion
        
        #region If fileType is Unknown at this point, determine based off of filename.
        if (fileType is EntryDataType.UnknownBinarized or EntryDataType.Unknown) {
            fileType = file.ToLower().Split(Path.DirectorySeparatorChar).Last() switch {
                "config.cpp" => EntryDataType.PoseidonConfig,
                "config.bin" => EntryDataType.BinarizedPoseidonConfig,
                "tex" + "headers.bin" => EntryDataType.BinarizedTexHeaders,
                "string" + "table.csv" => EntryDataType.StringTable,
                _ => EntryDataType.BinarizedRVMAT
            };
        }
        #endregion
        
        #region If fileType is PoseidonConfig, determine content type.
        // Since cpp files can contain both binarized and conventional content,
        // we have to peak the first four bytes to identify a rap file
        /*if (fileType is EntryDataType.PoseidonConfig) 
            fileType = new RapBinaryReader(File.ReadAllBytes(file)).ReadHeader()
            ? EntryDataType.BinarizedPoseidonConfig
            : EntryDataType.PoseidonConfig;
        */
        #endregion
    
        return fileType;
    }
}
 
internal static class PBOUtilities {
    // ReSharper disable once StringLiteralTypo
    private static string RecentFolderGUID = "{22877a6d-37a1-461a-91b0-dbda5aaebc99}";
    private const string BinarizeBatFileLocation = @"C:\Program Files (x86)\Steam\steamapps\common\DayZ Tools\Bin\CfgConvert\CPPToBIN.bat";
    
    private static readonly string[] IllegalWindowsFilenames = new[] {
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3",
        "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8",
        "LPT9"
    };

    public static string randomUnicode(int rangeStart, int rangeEnd, int length = 8) {

        var random = new Random(DateTime.Now.Millisecond);
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++) {
            var next = (char) (random.Next(rangeStart, rangeEnd + 1) %
                               (rangeEnd - rangeStart));
            if (next == '\0') i--;
            else {
                builder.Append(next);
            }
        }

        var ret = builder.ToString();
        //if (randoms.Contains(ret)) return randomUnicode(rangeStart, rangeEnd, length);
        //randoms.Add(ret);
        Console.Write(".");
        return ret;
    }

    public static string RandomizeStringCase(string someString) {
        var randomizer = new Random();
        var final =
            someString.Select(x => randomizer.Next() % 2 == 0 ? 
                (char.IsUpper(x) ? x.ToString().ToLower().First() : x.ToString().ToUpper().First()) : x);
        return new string(final.ToArray()); 
    }
    
    public static string randomChineseChar() {
        return randomChinese(1);
    }
    public static string randomChinese(int length = 8) {
        return randomUnicode(0x2B820, 0x2CEAF, length);
    }
    
    public static string GenerateObfuscatedPath(string filename = "") {
        var pathBuilder = new StringBuilder(RandomString(includeSpaces: true));
        pathBuilder.Append('\\').Append(RandomString(5)).Append("\\\\\\\\").Append(" .").Append(RecentFolderGUID).Append('\\').Append("..\\").Append(randomChinese()).Append("\\com1");
        if (filename != Empty) pathBuilder.Append('\\').Append(filename);
        return RandomizeStringCase(pathBuilder.ToString());
    }

    public static string RandomString(int stringLength = 8, string allowableChars = null, bool includeSpaces = false, bool includeNumbers = true) {
        // ReSharper disable once StringLiteralTypo
        if (IsNullOrEmpty(allowableChars)) allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        if (includeNumbers) allowableChars += "0123456789";
        if (includeSpaces) allowableChars += " ";

        
        var rnd = new byte[stringLength];
        using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(rnd);
        
        var allowable = allowableChars.ToCharArray();
        var l = allowable.Length;
        var chars = new char[stringLength];
        for (var i = 0; i < stringLength; i++) chars[i] = allowable[rnd[i] % l];

        var generatedString = new string(chars);
        
        //if (randoms.Contains(generatedString)) return RandomString(stringLength, allowableChars, includeNumbers);
        //randoms.Add(generatedString);
        //Console.Write(".");
        return generatedString;
    }
    
    public static string GetRelativePath(string folderPath, string filePath) {
        var separator = Path.DirectorySeparatorChar;

        //Make sure the folder path actually refers to a folder
        if (!folderPath.EndsWith(separator)) {
            folderPath += separator;
        }

        var cutoffPos = 0;
        var length = folderPath.Length;
        for (var i = length - 1; i >= 0; i--) {
            if (folderPath[i] != separator) continue;
            cutoffPos = i;
            break;
        }

        var subString = folderPath.Substring(0, cutoffPos + 1);

        return filePath.Replace(subString, "");
    }
    
    //TODO: Remove this shit and do binarization all by myself :)
    public static byte[] BinarizeConfig(string configPath) {
        var binnedOutputPath = configPath.Replace(".cpp", ".bin");

        var binTask = new Process() {
            StartInfo = new ProcessStartInfo {
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = false,
                FileName = "cmd.exe",
                Arguments = $"/C \"{BinarizeBatFileLocation}\" {configPath}"
            }
        };

        binTask.Start();
        binTask.WaitForExit();
                
        var binnedOutput = File.ReadAllBytes(binnedOutputPath);
        if(File.Exists(binnedOutputPath)) File.Delete(binnedOutputPath);
        return binnedOutput;
    }
}

