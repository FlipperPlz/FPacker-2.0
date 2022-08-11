using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.ODOL; 

public abstract class STPair {
    public Vector3P S { get; } = new();
    public Vector3P T { get; } = new();
}