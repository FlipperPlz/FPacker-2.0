using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapInt : BaseRapValue<PoseidonParser.LiteralIntegerContext, int> {
    public override string ToRapFormat() => Value.ToString();
    public override void FromRapFormat(PoseidonParser.LiteralIntegerContext ctx) => Value = int.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));

    public static RapInt FromParseContext(PoseidonParser.LiteralIntegerContext ctx) {
        var rInt = new RapInt();
        rInt.FromRapFormat(ctx);
        return rInt;
    }
}