using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.CPP.Parse;
using FPacker.Formats.RAP.Models;

namespace FPacker.Formats.CPP.Models; 

class ConfigFile {
    public string PBOPath { get; private set; }
    public string PBOConfigRoot { get; private set; }
    public string SystemPath { get; init; }
    public string SystemConfigRoot { get; private set; }
    public RapFile PoseidonConfig { get;  private set; }
    public List<AddonPrefix> PrefixObjs { get; private set; } = new();
    
    public List<string> ModelPaths { get; private set; }
    public List<string> TexturePaths { get; private set; }
    public List<string> MaterialPaths { get; private set; }
    public List<string> SoundSamplePaths { get; private set; }

    public List<string> FoundPaths = new();
    public Dictionary<string, string> ObfuscatedPaths { get; private set; } = new();


    public ConfigFile(string pboPath, string systemPath) {
        PBOPath = pboPath;
        SystemPath = systemPath;
        SystemConfigRoot = SystemPath.Replace(new FileInfo(systemPath).Name, "");
        PBOConfigRoot = PBOPath.Replace(new FileInfo(systemPath).Name, "");
        ParseConfig();
    }

    private void ParseConfig() {
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        PoseidonConfig = listener.ConfigFile;
        listener.EstablishedPrefixes.ForEach(p => PrefixObjs.Add(new AddonPrefix() {
            PrefixName = p,
            PBOPath = PBOConfigRoot,
            SystemPath = SystemConfigRoot
            
        }));
        ModelPaths = listener.ModelPaths;
        TexturePaths = listener.TexturePaths;
        MaterialPaths = listener.MaterialPaths;
        SoundSamplePaths = listener.SoundSamplePaths;
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
        var listener = new ConfigObfuscationListener() { Rewriter = rewriter, ObfuscatedPaths = ObfuscatedPaths };
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        
        ResetData(rewriter.GetText());
    }
    
    private void ResetData(string getText) {
        var lexer = new PoseidonLexer(CharStreams.fromString(getText));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        PoseidonConfig = listener.ConfigFile;
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