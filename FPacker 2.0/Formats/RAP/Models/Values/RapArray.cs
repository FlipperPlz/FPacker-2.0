using System.Text;
using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.P3D.IO;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapArray : RapValue<List<IRapValue>> {
    
    public int EntryCount;

    public RapArray() => Value = new List<IRapValue>(); 

    public RapArray(IEnumerable<string> strings) {
        Value = new List<IRapValue>();
        foreach (var str in strings) Value.Add(new RapString() {Value = str});
    }

    public override string ToRapFormat() => new StringBuilder("{").Append(string.Join(',', Value.Select(static v => v.ToRapFormat()))).Append('}').ToString();
    public override void ToBinaryContext(RapBinaryWriter writer, bool defaultFalse = false) {
        writer.WriteCompressedInt(Value.Count);
        if (EntryCount == 0) return;

        foreach (var val in Value) {
            writer.Write(val.Type());
            val.ToBinaryContext(writer);
        }
    }

    
    public List<RapString> FlattenToStrings(bool recursive = true) {
        var foundStrings = new List<RapString>();
        foreach (var value in Value) {
            switch (value) {
                case RapArray childArray:
                    if(recursive) foundStrings.AddRange(childArray.FlattenToStrings(recursive));
                    break;
                case RapString rapString:
                    foundStrings.Add(rapString);
                    break;
            }
        }

        return foundStrings;
    }
    
    public override Tself FromRapContext<Tself>(ParserRuleContext context) {
        if (context is not PoseidonParser.ArrayInitializerContext ctx) throw new Exception();
        foreach (var val in ctx.variableInitializer()) {
            if (val.arrayInitializer() is { } childArr) {
                Value.Add(new RapArray().FromRapContext<RapArray>(childArr));
                continue;
            } else if(val.literal().literalFloat() is { } literalFloat ) {
                Value.Add(new RapFloat().FromRapContext<RapFloat>(literalFloat));         
                continue;
            } else if(val.literal().literalInteger() is { } literalInteger ) {
                Value.Add(new RapInt().FromRapContext<RapInt>(literalInteger));         
                continue;
            } else if(val.literal().literalString() is { } literalString) {
                Value.Add(new RapString().FromRapContext<RapString>(literalString));         
                continue;
            } 
        }
        return (Tself) (IRapValue) this;
    }

    public override Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) {
        EntryCount = reader.ReadCompressedInteger();
        if (EntryCount == 0) return (Tself) (IRapValue) this;

        for (var i = 0; i < EntryCount; ++i) {
            switch ((RapValueType) reader.ReadByte()) {
                case RapValueType.String:
                    Value.Add(reader.ReadBinarizedValue<RapString>());
                    break;
                case RapValueType.Float:
                    Value.Add(reader.ReadBinarizedValue<RapFloat>());
                    break;
                case RapValueType.Long:
                    Value.Add(reader.ReadBinarizedValue<RapInt>());
                    break;
                case RapValueType.Array:
                    Value.Add(reader.ReadBinarizedValue<RapArray>());
                    break;
                case RapValueType.Variable: throw new NotImplementedException("raP variables are not supported.");
                default: throw new Exception("How the fuck did you get here?");
            }
        }

        return (Tself) (IRapValue) this;
    }

    public override byte Type() => (byte) RapValueType.Array;
}