using System.Globalization;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapFloat : BaseRapValue<PoseidonParser.LiteralFloatContext, float> {
    public RapFloat(float f) => Value = f;
    private RapFloat() { }
    public override string ToRapFormat() => Value.ToString(CultureInfo.CurrentCulture);
    public override void FromRapContext(PoseidonParser.LiteralFloatContext ctx) => Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
   
    public static RapFloat FromParseContext(PoseidonParser.LiteralFloatContext ctx) {
        var rFloat = new RapFloat();
        rFloat.FromRapContext(ctx);
        return rFloat;
    }
}