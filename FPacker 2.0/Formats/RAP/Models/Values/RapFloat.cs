using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapFloat : RapValue<float> {
    public RapFloat(float f) => Value = f;
    public RapFloat() { }
    public override string ToRapFormat() => Value.ToString(CultureInfo.CurrentCulture);
    public override void ToBinaryContext(RapBinaryWriter writer) => writer.Write((float) Value);

    public override U FromRapContext<U>(ParserRuleContext ctx) {
        if (ctx is not PoseidonParser.LiteralFloatContext) throw new Exception();
        Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return (U) (IRapValue) this;
    }

    public override U FromBinaryContext<U>(RapBinaryReader reader, bool defaultFalse = false) {
        Value = reader.ReadSingle();
        return (U) (IRapValue) this;
    }

}