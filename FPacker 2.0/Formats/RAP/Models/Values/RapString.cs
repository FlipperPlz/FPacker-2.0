using System.Text;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapString : BaseRapValue<PoseidonParser.LiteralStringContext, string> {
    public override string ToRapFormat() => new StringBuilder("\"").Append(Value).Append('"').ToString();
    public override void FromRapFormat(PoseidonParser.LiteralStringContext ctx) => Value = ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)).TrimStart('"').TrimEnd('"');

    public static RapString FromParseContext(PoseidonParser.LiteralStringContext ctx) {
        var str = new RapString();
        str.FromRapFormat(ctx);
        return str;
    }

}
