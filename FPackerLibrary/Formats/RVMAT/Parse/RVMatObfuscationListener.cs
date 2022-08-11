using Antlr4.Runtime;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.Models.Values;

namespace FPackerLibrary.Formats.RVMAT.Parse;

public class RVMatObfuscationListener : PoseidonBaseListener {
    internal TokenStreamRewriter Rewriter { get; init; }
    internal Dictionary<string, string> ObfuscatedPaths { get; init; } = new();

    public override void ExitLiteralString(PoseidonParser.LiteralStringContext context) {
        var str = new RapString().FromRapContext<RapString>(context);
        if (ObfuscatedPaths.ContainsKey(str.Value)) {
            Rewriter.Replace(context.Start, context.Stop, $"\"{ObfuscatedPaths[str.Value]}\"");
            
        }

        base.ExitLiteralString(context);
    }
}