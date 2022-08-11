using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.IO;
using FPackerLibrary.Formats.RAP.Models.Enums;

namespace FPackerLibrary.Formats.RAP.Models.Values; 

public class RapString : RapValue<string> {
    public override string ToRapFormat() => new StringBuilder("\"").Append(Value).Append('"').ToString();
    public override void ToBinaryContext(RapBinaryWriter writer, bool defaultFalse = false) => writer.WriteAsciiZ(Value);
    
    public RapString(string str) => Value = str;
    public RapString() {}
    
    public override U FromRapContext<U>(ParserRuleContext ctx) {
        if (ctx is not PoseidonParser.LiteralStringContext) throw new Exception();
        Value = ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)).TrimStart('"').TrimEnd('"');
        return (U) (IRapValue) this;
    }

    public override U FromBinaryContext<U>(RapBinaryReader reader, bool defaultFalse = false) {
        Value = reader.ReadAsciiZ();
        return (U) (IRapValue) this;
    }
    public override byte Type() => (byte) RapValueType.String;

}
