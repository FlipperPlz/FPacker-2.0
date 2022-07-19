using System.Text;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapArray : BaseRapValue<PoseidonParser.ArrayInitializerContext, List<IRapSerializable>> {
    public int EntryCount;

    public RapArray() => Value = new List<IRapSerializable>(); 

    public RapArray(IEnumerable<string> strings) {
        Value = new List<IRapSerializable>();
        foreach (var str in strings) Value.Add(new RapString() {Value = str});
    }

    public override string ToRapFormat() => new StringBuilder("{").Append(string.Join(',', Value.Select(static v => v.ToRapFormat()))).Append('}').ToString();

    public static RapArray FromParseContext(PoseidonParser.ArrayInitializerContext ctx) {
        var rArray = new RapArray();
        foreach (var val in ctx.variableInitializer()) {
            if (val.arrayInitializer() is { } childArr) {
                rArray.Value.Add(RapArray.FromParseContext(childArr));
                continue;
            } else if(val.literal().literalFloat() is { } literalFloat ) {
                rArray.Value.Add(RapFloat.FromParseContext(literalFloat));         
                continue;
            } else if(val.literal().literalInteger() is { } literalInteger ) {
                rArray.Value.Add(RapInt.FromParseContext(literalInteger));         
                continue;
            } else if(val.literal().literalString() is { } literalString) {
                rArray.Value.Add(RapString.FromParseContext(literalString));         
                continue;
            } 
        }
        return rArray;
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
    
    public override void FromRapContext(PoseidonParser.ArrayInitializerContext ctx) => FromParseContext(ctx);
}