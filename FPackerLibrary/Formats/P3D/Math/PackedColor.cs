namespace FPackerLibrary.P3D.Math; 

public struct PackedColor {
    public uint Value;
    public byte A8 => (byte)(this.Value >> 24 & 255U);
    public byte R8 => (byte)(this.Value >> 16 & 255U);
    public byte G8 => (byte)(this.Value >> 8 & 255U);
    public byte B8 => (byte)(this.Value & 255U);
    
    public PackedColor(uint value) => Value = value;
    
    public PackedColor(byte r, byte g, byte b, byte a = 255) => Value = PackColor(r, g, b, a);
    
    public PackedColor(float r, float g, float b, float a) {
        var r2 = (byte)(r * 255f);
        var g2 = (byte)(g * 255f);
        var b2 = (byte)(b * 255f);
        var a2 = (byte)(a * 255f);
        Value = PackColor(r2, g2, b2, a2);
    }

    private static uint PackColor(byte r, byte g, byte b, byte a) => (uint)(a << 24 | r << 16 | g << 8 | b);

}