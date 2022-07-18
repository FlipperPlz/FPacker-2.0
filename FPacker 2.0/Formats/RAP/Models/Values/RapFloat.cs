using System.Globalization;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapFloat : BaseRapValue<PoseidonParser.LiteralFloatContext, float> {
    public override string ToRapFormat() => Value.ToString(CultureInfo.CurrentCulture);
    public override void FromRapFormat(PoseidonParser.LiteralFloatContext ctx) => Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
   
    public static RapFloat FromParseContext(PoseidonParser.LiteralFloatContext ctx) {
        var rFloat = new RapFloat();
        rFloat.FromRapFormat(ctx);
        return rFloat;
    }
}