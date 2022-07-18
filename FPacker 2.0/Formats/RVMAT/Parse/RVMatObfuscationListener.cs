using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RVMAT.Parse;

public class RVMatObfuscationListener : PoseidonBaseListener {
    internal TokenStreamRewriter Rewriter { get; init; }
    internal Dictionary<string, string> ObfuscatedPaths { get; init; } = new();

    public override void ExitLiteralString(PoseidonParser.LiteralStringContext context) {
        var str = RapString.FromParseContext(context);
        if (ObfuscatedPaths.ContainsKey(str.Value)) {
            Rewriter.Replace(context.Start, context.Stop, $"\"{ObfuscatedPaths[str.Value]}\"");
            
        }

        base.ExitLiteralString(context);
    }
}