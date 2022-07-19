using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models.Values; 


public abstract class BaseRapValue<TCtx, VType> : IRapSerializable, IRapValue {
    public VType Value { get; set; }
    public abstract string ToRapFormat();
    public abstract void FromRapContext(TCtx ctx);
    protected BaseRapValue(VType value) => Value = value;

    protected BaseRapValue() { }
}

public interface IRapValue { };