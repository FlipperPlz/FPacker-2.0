using FPacker.P3D.IO;
using NLog.Targets;

namespace FPacker.P3D.Math;

public class Vector3PCompressed : IDeserializable {
    private int _value;
    private const float ScaleFactor = -0.0019569471f;
    public float X {
        get {
            var num = this._value & 1023;
            if (num > 511) num -= 1024;
            return num * -0.0019569471f;
        }
    }

    public float Y {
        get {
            var num = _value >> 10 & 1023;
            if (num > 511) num -= 1024;
            return num * -0.0019569471f;
        }
    }

    public float Z {
        get {
            var num = _value >> 20 & 1023;
            if (num > 511) num -= 1024;
            return num * -0.0019569471f;
        }
    }

    public static implicit operator Vector3P(Vector3PCompressed src) {
        var num = src._value & 1023;
        var num2 = src._value >> 10 & 1023;
        var num3 = src._value >> 20 & 1023;
        if (num > 511) num -= 1024;
        if (num2 > 511) num2 -= 1024;
        if (num3 > 511) num3 -= 1024;
        return new Vector3P((float) num * -0.0019569471f, (float) num2 * -0.0019569471f, (float) num3 * -0.0019569471f);
    }

    public static implicit operator int(Vector3PCompressed src) => src._value;

    public static implicit operator Vector3PCompressed(int src) => new() {_value = src};

    public void ReadObject(P3DBinaryReader input) => _value = input.ReadInt32();
    public void WriteObject(P3DBinaryWriter output) => output.WriteInt32(_value);
}