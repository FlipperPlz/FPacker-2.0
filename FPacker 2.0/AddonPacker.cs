using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FPacker.Formats.CPP.Models;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RVMAT.Models;
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

struct AddonPrefix {
    public string SystemPath { get; set; } //This is the prefix root established by where the config is
    public string PBOPath; //This is the pbo path to the directory containing that prefix
    public string PrefixName;
}



struct AddonFile {
    public string PBOPath { get; set; }
    public string PBOReferencePath { get; set; }
    public string SystemPath { get; set; }
    public string PBOObfuscatedPath { get; set; }
    public string PBOObfuscatedRefPath { get; set; }

    public EntryDataType Type { get; set; }
}

internal class AddonPacker {

    private static readonly Logger Logger = LogManager.GetLogger("FPacker");
    
    private readonly string _srcFolder;
    private readonly List<AddonPrefix> _prefixes = new();
    private readonly List<AddonFile> _files = new();
    
    private readonly Dictionary<string, string> _obfuscatedPaths = new();
    
    private readonly List<string> foundPaths = new();
    private readonly List<string> _foundRvMatPaths = new();
    private readonly List<string> _foundModelPaths = new();

    
    private readonly List<ConfigFile> _configs = new();
    private readonly List<RVMatFile> _materials = new();
    private readonly List<P3DFile> _models = new();


    public AddonPacker(string sourceFolder, string outPath) {
        InitializeLogger();
        Logger.Info("Parsing configs in {srcFolder}.", sourceFolder);

        #region Parse all config.cpp files and gather referenced paths.
        foreach (var file in new DirectoryInfo(sourceFolder).GetFiles("config.*", SearchOption.AllDirectories)) {
            Logger.Info("Parsing config: {fileName}", file.FullName);
            var pboPath = PBOUtilities.GetRelativePath(sourceFolder, file.FullName);
            var cfg = new ConfigFile(pboPath, file.FullName);
            _configs.Add(cfg);
            _prefixes.AddRange(cfg.PrefixObjs);
            foreach (var modelPath in cfg.ModelPaths) {
                if(!foundPaths.Contains(modelPath)) foundPaths.Add(modelPath);
                if(!_foundModelPaths.Contains(modelPath)) _foundModelPaths.Add(modelPath);
            }
            foreach (var texturePath in cfg.TexturePaths.Where(texturePath => !foundPaths.Contains(texturePath))) foundPaths.Add(texturePath);
            foreach (var rvmatPath in cfg.MaterialPaths) {
                if(!foundPaths.Contains(rvmatPath)) foundPaths.Add(rvmatPath);
                if(!_foundRvMatPaths.Contains(rvmatPath)) _foundRvMatPaths.Add(rvmatPath);
            }
            foreach (var samplePath in cfg.SoundSamplePaths.Where(samplePath => !foundPaths.Contains(samplePath))) foundPaths.Add(samplePath);
        }
        
        #endregion

        foreach (var modelPath in _foundModelPaths) {
            var pboRefPath = SanitizePBOPath(modelPath);
            var systemPath = ConvertPBORefPath2SystemPath(pboRefPath);
            if(systemPath is null || !File.Exists(systemPath)) continue;
            var pboPath = ConvertPBORefPath2PBOPath(SanitizePBOPath(modelPath));
            Logger.Info("Parsing P3D: {sysPath}", systemPath);
            var p3dFile = new P3DFile(pboPath, pboRefPath, systemPath);
            _models.Add(p3dFile);
            foreach (var texturePath in p3dFile.TexturePaths.Where(texturePath => !foundPaths.Contains(texturePath))) foundPaths.Add(texturePath);
            foreach (var rvmatPath in p3dFile.MaterialPaths) {
                if(!foundPaths.Contains(rvmatPath)) foundPaths.Add(rvmatPath);
                if(!_foundRvMatPaths.Contains(rvmatPath)) _foundRvMatPaths.Add(rvmatPath);
            }
            foreach (var surfacePath in p3dFile.SurfacePaths.Where(surfacePath => !foundPaths.Contains(surfacePath))) foundPaths.Add(surfacePath);

        }
        
        foreach (var rvMatPath in _foundRvMatPaths) {
            var pboRefPath = SanitizePBOPath(rvMatPath);
            var systemPath = ConvertPBORefPath2SystemPath(pboRefPath);
            if(systemPath is null || !File.Exists(systemPath)) continue;
            var pboPath = ConvertPBORefPath2PBOPath(SanitizePBOPath(rvMatPath));
            Logger.Info("Parsing RVMat: {sysPath}", systemPath);
            var rvmat = new RVMatFile(pboPath, pboRefPath, systemPath);
            _materials.Add(rvmat);
            foundPaths.AddRange(rvmat.TexturePaths);
        }

        foreach (var foundPath in foundPaths) {
            var sanitizedRefPath = SanitizePBOPath(foundPath);
            var systemPath = ConvertPBORefPath2SystemPath(sanitizedRefPath);
            if(systemPath is null || !File.Exists(systemPath)) continue;
            Logger.Debug("Located system path for {pboref}", sanitizedRefPath);
            if (!_obfuscatedPaths.TryGetValue(sanitizedRefPath, out var obfuscatedPBORefPath)) {
                obfuscatedPBORefPath = DeterminePrefixFromPath(sanitizedRefPath).PrefixName + Path.DirectorySeparatorChar + PBOUtilities.RandomString(25, includeSpaces: true, includeNumbers: false) +
                    '.' + sanitizedRefPath.Split('.').Last();
            }
            _obfuscatedPaths.TryAdd(sanitizedRefPath, obfuscatedPBORefPath);

             _obfuscatedPaths.TryAdd(foundPath, obfuscatedPBORefPath);
            obfuscatedPBORefPath = string.Empty;
        }

        foreach (var p3DFile in _models) {
            foreach (var proxyPath in p3DFile.ProxyPaths) {
                if(_obfuscatedPaths.ContainsKey(proxyPath) || proxyPath.ToLower().StartsWith("dz")) continue;
                var sanitizedRefPath = SanitizePBOPath(Path.ChangeExtension(proxyPath, ".p3d"));
                var proxySysPath = ConvertPBORefPath2SystemPath(sanitizedRefPath);
                if(proxySysPath is null || !File.Exists(proxySysPath)) continue;
                Logger.Info("Located proxy for {p}", p3DFile.PBOPath);
                if (!_obfuscatedPaths.TryGetValue(sanitizedRefPath, out var obfuscatedPBORefPath)) {
                    obfuscatedPBORefPath = DeterminePrefixFromPath(sanitizedRefPath).PrefixName + Path.DirectorySeparatorChar + PBOUtilities.RandomString(25, includeSpaces: true, includeNumbers: false) +
                                           '.' + sanitizedRefPath.Split('.').Last();
                }

                _obfuscatedPaths.TryAdd(proxyPath, obfuscatedPBORefPath);
                _obfuscatedPaths.TryAdd(sanitizedRefPath, obfuscatedPBORefPath);

                obfuscatedPBORefPath = string.Empty;
            }
        }
        
        
        foreach (var mat in _materials) {
            Logger.Info("Obfuscating RVMAT: {pboFile}", mat.PBOPath);
            foreach (var foundPath in mat.TexturePaths.Where(foundPath => _obfuscatedPaths.ContainsKey(foundPath))) mat.ObfuscatedPaths.Add(foundPath, _obfuscatedPaths[foundPath]);
            mat.ObfuscatePaths(ConvertPBORefPath2PBOPath(_obfuscatedPaths[mat.PBOReferencePath]), _obfuscatedPaths[mat.PBOReferencePath]);
        }

        foreach (var cfg in _configs) {
            Logger.Info("Obfuscating config: {pboFile}", cfg.PBOPath);

            foreach (var foundPath in cfg.FoundPaths.Where(foundPath => _obfuscatedPaths.ContainsKey(foundPath))) cfg.ObfuscatedPaths.Add(foundPath, _obfuscatedPaths[foundPath]);
            cfg.ObfuscatePaths(PBOUtilities.RandomString(25, includeSpaces: true, includeNumbers: false) + Path.DirectorySeparatorChar + "config.cpp");
        }

        foreach (var model in _models) {
            Logger.Info("Obfuscating P3D: {pboFile}", model.PBOPath);
            foreach (var foundPath in model.FoundPaths.Where(foundPath => _obfuscatedPaths.ContainsKey(foundPath))) model.ObfuscatedPaths.Add(foundPath, _obfuscatedPaths[foundPath]);
            model.ObfuscatePaths(ConvertPBORefPath2PBOPath(_obfuscatedPaths[model.PBOReferencePath]), _obfuscatedPaths[model.PBOReferencePath]);
        }

        var entries = new List<PBOEntry>();
        entries.AddRange(_materials.Select(static materialFile => new PBOEntry(materialFile.ObfuscatedPBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.RVMAT, Encoding.UTF8.GetBytes(materialFile.RVMatData.ToRapFormat()), materialFile.SystemPath)));
        entries.AddRange(_configs.Select(static configFile => new PBOEntry(configFile.PBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.PoseidonConfig, Encoding.UTF8.GetBytes(configFile.PoseidonConfig.ToRapFormat()), configFile.SystemPath)));
        foreach (var model in _models) {
            if (model.P3DData is ODOL odol) {
                //TODO: Write ODOL
                entries.Add(new PBOEntry(model.PBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.Model, FormatConversion.Covert2MLOD(odol).WriteToMemory().ToArray()));
            } else if (model.P3DData is MLOD mlod) {
                entries.Add(new PBOEntry(model.PBOPath, (int) EntryPackingType.Uncompressed, EntryDataType.Model, mlod.WriteToMemory().ToArray()));

            }
        }

        Logger.Info("Starting packing process for {n} files", entries.Count);
        new PBOStream(File.Create(outPath)).WritePBO(entries);

        Logger.Log(LogLevel.Info, JsonSerializer.Serialize(_prefixes.Select(static p => p.PrefixName)));
    }

    private void InitializeLogger() {
        NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Users\dev\Desktop\FPacker 2.0\FPacker 2.0\NLog.config",true);
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
        if (!includeSpaces) allowableChars += "      \t\t\n";

        
        var rnd = new byte[stringLength];
        using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(rnd);
        
        var allowable = allowableChars.ToCharArray();
        var l = allowable.Length;
        var chars = new char[stringLength];
        for (var i = 0; i < stringLength; i++)
            chars[i] = allowable[rnd[i] % l];

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

