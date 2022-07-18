using System.Globalization;
using FPacker.P3D.IO;

namespace FPacker.P3D.Math; 

public class Vector3P : IDeserializable {
    
    private readonly float[] _coordinates;

    private int _compressedAs; 
    
    public float X {
        get => _coordinates[0];
        set => _coordinates[0] = value;
    }
    
    public float Y {
        get => _coordinates[1];
        set => _coordinates[1] = value;
    }
    
    public float Z {
        get => _coordinates[2];
        set => _coordinates[2] = value;
    }
    
    public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);
    
    public float this[int i] {
        get => _coordinates[i];
        set => _coordinates[i] = value;
    }

    
    public Vector3P() : this(0f) { }
    public Vector3P(float val) : this(val, val, val) { }
    public Vector3P(P3DBinaryReader input) : this(input.ReadSingle(), input.ReadSingle(), input.ReadSingle()) { }
    public Vector3P(float x, float y, float z) => _coordinates = new [] {x, y, z};
    
    
    public static Vector3P operator *(Vector3P a, float b) => new(a.X * b, a.Y * b, a.Z * b);
    public static float operator *(Vector3P a, Vector3P b) =>  a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    public static Vector3P operator +(Vector3P a, Vector3P b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3P operator -(Vector3P a) => new(0f - a.X, 0f - a.Y, 0f - a.Z);
    public static Vector3P operator -(Vector3P a, Vector3P b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    
    public override bool Equals(object obj) => obj is Vector3P vector3P && base.Equals(obj) && Equals(vector3P);
    
    public override int GetHashCode() => base.GetHashCode();
    
    public bool Equals(Vector3P other) {
        var func = static (float f1, float f2) => (double)System.Math.Abs(f1 - f2) < 0.05;
        return func(X, other.X) && func(Y, other.Y) && func(Z, other.Z);
    }
    
    public override string ToString() {
        var cultureInfo = new CultureInfo("en-US");
        return string.Concat("{", X.ToString(cultureInfo.NumberFormat), ",", Y.ToString(cultureInfo.NumberFormat), ",", Z.ToString(cultureInfo.NumberFormat), "}");
    }
    
    public void ReadCompressed(P3DBinaryReader input) {
        var num = _compressedAs = input.ReadInt32();
        var num2 = num & 1023;
        var num3 = num >> 10 & 1023;
        var num4 = num >> 20 & 1023;
        if (num2 > 511) num2 -= 1024;
        if (num3 > 511) num3 -= 1024;
        if (num4 > 511) num4 -= 1024;
        X = (float)(num2 * -0.0019569471624266144);
        Y = (float)(num3 * -0.0019569471624266144);
        Z = (float)(num4 * -0.0019569471624266144);
    }

    public void WriteCompressed(P3DBinaryWriter output) {
        output.WriteInt32(_compressedAs);
    }
    
    public void WriteObject(P3DBinaryWriter output) {
        output.Write(X);
        output.Write(Y);
        output.Write(Z);
    }
    
    public float Distance(Vector3P v) {
        var vector3P = this - v;
        return (float)System.Math.Sqrt((vector3P.X * vector3P.X + vector3P.Y * vector3P.Y + vector3P.Z * vector3P.Z));
    }
    
    public void Normalize() {
        var num = (float)Length;
        X /= num;
        Y /= num;
        Z /= num;
    }

    public void ReadObject(P3DBinaryReader input) {
        _coordinates[0] = input.ReadSingle();
        _coordinates[1] = input.ReadSingle();
        _coordinates[2] = input.ReadSingle();
    }
} 