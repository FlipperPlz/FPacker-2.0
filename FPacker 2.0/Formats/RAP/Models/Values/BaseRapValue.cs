using Antlr4.Runtime;
using FPacker.Formats.RAP.IO;
using FPacker.P3D.IO;

namespace FPacker.Formats.RAP.Models.Values; 


public abstract class RapValue<T> : IRapValue {
    public abstract string ToRapFormat();
    public abstract void ToBinaryContext(RapBinaryWriter writer);
    protected RapValue() { }
    protected RapValue(T value) => Value = value;
    public abstract U FromRapContext<U>(ParserRuleContext ctx) where U : IRapDeserializable;
    public abstract U FromBinaryContext<U>(RapBinaryReader reader, bool defaultFalse = false) where U : IRapDeserializable;
    public T Value { get; set; }
}

public interface IRapValue: IRapDeserializable {
}