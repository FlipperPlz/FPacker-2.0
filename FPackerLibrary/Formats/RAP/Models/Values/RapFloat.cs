using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.IO;
using FPackerLibrary.Formats.RAP.Models.Enums;

namespace FPackerLibrary.Formats.RAP.Models.Values; 

public class RapFloat : RapValue<float> {
    public RapFloat(float f) => Value = f;
    public RapFloat() { }
    public override string ToRapFormat() => Value.ToString(CultureInfo.CurrentCulture);
    public override void ToBinaryContext(RapBinaryWriter writer, bool defaultFalse = false) => writer.Write((float) Value);

    public override U FromRapContext<U>(ParserRuleContext ctx) {
        if (ctx is not PoseidonParser.LiteralFloatContext) throw new Exception();
        Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return (U) (IRapValue) this;
    }

    public override U FromBinaryContext<U>(RapBinaryReader reader, bool defaultFalse = false) {
        Value = reader.ReadSingle();
        return (U) (IRapValue) this;
    }
    public override byte Type() => (byte) RapValueType.Float;

}