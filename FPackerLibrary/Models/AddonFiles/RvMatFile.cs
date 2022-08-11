using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.Formats.RVMAT.Parse;
using FPackerLibrary.Models;

namespace FPackerLibrary.Models.AddonFiles; 

public class RvMatFile : BaseAddonFileSerializable<RapFile> {

    public List<string> TexturePaths { get; private set; }
    public Dictionary<string, string> ObfuscatedPaths { get; set; } = new();
    
    public RvMatFile(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath, systemPath) { }


    public void ObfuscatePaths(string obfPBOPath, string obfPBORefPath) {
        ObfuscatedPBOPath = obfPBOPath;
        ObfuscatedPBORefPath = obfPBORefPath;
        
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var tokens = new CommonTokenStream(lexer);
        var parser = new PoseidonParser(tokens);
        var rewriter = new TokenStreamRewriter(tokens);
        var listener = new RVMatObfuscationListener() { Rewriter = rewriter, ObfuscatedPaths = ObfuscatedPaths };
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ResetData(rewriter.GetText());
    }

    private void ResetData(string getText) {
        var lexer = new PoseidonLexer(CharStreams.fromString(getText));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new RVMatPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.RvmatFile;
        TexturePaths = listener.TexturePaths;
    }

    protected override void ParseObject(Stream data) {
        var lexer = new PoseidonLexer(CharStreams.fromStream(data));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new RVMatPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.RvmatFile;
        TexturePaths = listener.TexturePaths;
    }

}