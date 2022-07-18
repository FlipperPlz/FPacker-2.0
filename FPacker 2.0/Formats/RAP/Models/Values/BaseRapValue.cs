namespace FPacker.Formats.RAP.Models.Values; 

public abstract class BaseRapValue<TCtx, VType> : RapSerializable {
    public VType Value { get; set; }
    public abstract string ToRapFormat();
    public abstract void FromRapFormat(TCtx ctx);
    
    protected BaseRapValue(VType value) => Value = value;

    protected BaseRapValue() { }
}
