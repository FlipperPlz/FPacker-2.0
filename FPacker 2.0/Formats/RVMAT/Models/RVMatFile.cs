using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RVMAT.Parse;

namespace FPacker.Formats.RVMAT.Models; 

public class RVMatFile {
    public string PBOPath { get; init; }
    public string PBOReferencePath { get; init; }
    public string ObfuscatedPBOPath { get; private set; }
    public string ObfuscatedPBORefPath { get; private set; }

    public string SystemPath { get; init; }
    
    public RapFile RVMatData { get; private set; }
    
    public List<string> TexturePaths { get; private set; }
    public Dictionary<string, string> ObfuscatedPaths { get; set; } = new();

    
    public RVMatFile(string pboPath, string pboRefPath, string systemPath) {
        PBOPath = pboPath;
        PBOReferencePath = pboRefPath;
        SystemPath = systemPath;
        ParseRVMat();
    }

    private void ParseRVMat() {
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new RVMatPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        RVMatData = listener.RvmatFile;
        TexturePaths = listener.TexturePaths;
    }

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
        RVMatData = listener.RvmatFile;
        TexturePaths = listener.TexturePaths;
    }
}