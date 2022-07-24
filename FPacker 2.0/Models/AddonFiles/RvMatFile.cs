using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RVMAT.Parse;

namespace FPacker.Models.AddonFiles; 

public class RvMatFile : BaseAddonFileSerializable<RapFile> {
    public List<string> TexturePaths { get; private set; }
    public Dictionary<string, string> ObfuscatedPaths { get; set; } = new();
    
    public RvMatFile(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath, systemPath) { }
    
    protected override void InitializeObject(Stream stream) {
        var lexer = new PoseidonLexer(CharStreams.fromStream(stream));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new RVMatPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.RvmatFile;
        TexturePaths = listener.TexturePaths;
    }

    public override void ObfuscateObject() {
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var tokens = new CommonTokenStream(lexer);
        var parser = new PoseidonParser(tokens);
        var rewriter = new TokenStreamRewriter(tokens);
        var listener = new RVMatObfuscationListener() { Rewriter = rewriter, ObfuscatedPaths = ObfuscatedPaths };
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = RapFile.FromString(rewriter.GetText());
    }
}