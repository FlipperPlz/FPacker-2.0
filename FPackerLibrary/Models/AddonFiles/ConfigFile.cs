using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.CPP.Parse;
using FPackerLibrary.Formats.Enforce.Models;
using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.Models;

namespace FPackerLibrary.Models.AddonFiles; 

class ConfigFileSerializable : BaseAddonFileSerializable<RapFile> {
    public string PBOConfigRoot { get; private set; }
    public string SystemConfigRoot { get; private set; }
    public string ObfuscatedPBOConfigRoot { get; private set; }

    
    public List<AddonPrefix> PrefixObjs { get; private set; } = new();
    public List<string> ModelPaths { get; private set; }
    public List<string> TexturePaths { get; private set; }
    public List<string> MaterialPaths { get; private set; }
    public List<string> SoundSamplePaths { get; private set; }

    
    public List<string> EngineScriptPaths = new();
    public List<string> GameLibScriptPaths = new();
    public List<string> GameScriptPaths = new();
    public List<string> WorldScriptPaths = new();
    public List<string> MissionScriptPaths = new();
    
    public List<string> FoundPaths = new();
    
    public List<EnforceFileSerializable> ReferencedScripts = new();



    public Dictionary<string, string> ObfuscatedPaths { get; private set; } = new();

    public ConfigFileSerializable(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath, systemPath) {
    }

    
    protected override void ParseObject(Stream stream) {
        var lexer = new PoseidonLexer(CharStreams.fromStream(stream));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.ConfigFile;
        PBOConfigRoot = PBOPath.Replace(new FileInfo(SystemPath).Name, "");
        SystemConfigRoot = SystemPath.Replace(new FileInfo(SystemPath).Name, "");
        listener.EstablishedPrefixes.ForEach(p => PrefixObjs.Add(new AddonPrefix() {
            PrefixName = p,
            PBOPath = PBOConfigRoot,
            SystemPath = SystemConfigRoot
            
        }));
        ModelPaths = listener.ModelPaths;
        TexturePaths = listener.TexturePaths;
        MaterialPaths = listener.MaterialPaths;
        SoundSamplePaths = listener.SoundSamplePaths;
        
        EngineScriptPaths = listener.EngineScriptPaths;
        GameLibScriptPaths = listener.GameLibScriptPaths;
        GameScriptPaths = listener.GameScriptPaths;
        WorldScriptPaths = listener.WorldScriptPaths;
        MissionScriptPaths = listener.MissionScriptPaths;

        FoundPaths.AddRange(ModelPaths);
        FoundPaths.AddRange(TexturePaths);
        FoundPaths.AddRange(MaterialPaths);
        FoundPaths.AddRange(SoundSamplePaths);

    }


    public void ObfuscatePaths(string newPath) {
        if(!PBOPath.StartsWith("config")) PBOPath = newPath;
        
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var tokens = new CommonTokenStream(lexer);
        var parser = new PoseidonParser(tokens);
        var rewriter = new TokenStreamRewriter(tokens);
        var listener = new ConfigObfuscationListener() { Rewriter = rewriter, ObfuscatedPaths = ObfuscatedPaths, EnforceFiles = ReferencedScripts};
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        
        ResetData(rewriter.GetText());
    }
    
    private void ResetData(string getText) {
        var lexer = new PoseidonLexer(CharStreams.fromString(getText));
        var tokens = new CommonTokenStream(lexer);

        var parser = new PoseidonParser(tokens);

        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.ConfigFile;
        ModelPaths = listener.ModelPaths;
        TexturePaths = listener.TexturePaths;
        MaterialPaths = listener.MaterialPaths;
        SoundSamplePaths = listener.SoundSamplePaths;
        FoundPaths.AddRange(ModelPaths);
        FoundPaths.AddRange(TexturePaths);
        FoundPaths.AddRange(MaterialPaths);
        FoundPaths.AddRange(SoundSamplePaths);
    }

}