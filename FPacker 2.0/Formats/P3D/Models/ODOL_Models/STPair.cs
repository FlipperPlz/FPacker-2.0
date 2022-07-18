using FPacker.P3D.Math;

namespace FPacker.P3D.Models.ODOL; 

public abstract class STPair {
    public Vector3P S { get; } = new();
    public Vector3P T { get; } = new();
}