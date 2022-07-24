using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.CPP.Parse;
using FPacker.Formats.Enforce.Models;
using FPacker.Formats.RAP.Models;
using FPacker.Models;

namespace FPacker.Models.AddonFiles; 

public class ConfigFile : BaseAddonFileSerializable<RapFile> {
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



    public Dictionary<string, string> ObfuscatedPaths { get; private set; } = new();

    public ConfigFile(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath, systemPath) {
    }

    
    protected override void InitializeObject(Stream fileStream) {
        var lexer = new PoseidonLexer(CharStreams.fromStream(RapFile.OpenStream(fileStream).WriteToStream()));
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
    
    public override void ObfuscateObject() {
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var tokens = new CommonTokenStream(lexer);
        var parser = new PoseidonParser(tokens);
        var rewriter = new TokenStreamRewriter(tokens);
        var listener = new ConfigObfuscationListener() { Rewriter = rewriter, ObfuscatedPaths = ObfuscatedPaths};
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = RapFile.FromString(rewriter.GetText());
    }

}