using System.Runtime.CompilerServices;
using System.Text;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models.Values; 

public class RapArray : BaseRapValue<PoseidonParser.ArrayInitializerContext, List<RapSerializable>> {
    public override string ToRapFormat() => new StringBuilder("{").Append(string.Join(',', Value.Select(static v => v.ToRapFormat()))).Append('}').ToString();
    public override void FromRapFormat(PoseidonParser.ArrayInitializerContext ctx) {
        Value = new List<RapSerializable>();
        foreach (var val in ctx.variableInitializer()) {
            if (val.arrayInitializer() is { } childArr) {
                Value.Add(RapArray.FromParseContext(childArr));
                continue;
            } else if(val.literal().literalFloat() is { } literalFloat ) {
                Value.Add(RapFloat.FromParseContext(literalFloat));         
                continue;
            } else if(val.literal().literalInteger() is { } literalInteger ) {
                Value.Add(RapInt.FromParseContext(literalInteger));         
                continue;
            } else if(val.literal().literalString() is { } literalString) {
                Value.Add(RapString.FromParseContext(literalString));         
                continue;
            } 
        }
    }
    
    public static RapArray FromParseContext(PoseidonParser.ArrayInitializerContext ctx) {
        var rArray = new RapArray();
        rArray.FromRapFormat(ctx);
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
}