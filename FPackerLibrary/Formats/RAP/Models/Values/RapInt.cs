using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapInt : RapValue<int> {
    public RapInt(int i) => Value = i;

    public RapInt() { }

    public override string ToRapFormat() => Value.ToString();
    
    public override void ToBinaryContext(RapBinaryWriter writer, bool defaultFalse = false) => writer.Write((int) Value);

    public override U FromRapContext<U>(ParserRuleContext context) {
        if(context is not PoseidonParser.LiteralIntegerContext ctx) throw new Exception();
        Value = int.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return (U) (IRapValue) this;
    }

    public override byte Type() => (byte) RapValueType.Long;

    
    public override U FromBinaryContext<U>(RapBinaryReader reader, bool defaultFalse = false) {
        Value = reader.ReadInt32();
        return (U) (IRapValue) this;
    }
}