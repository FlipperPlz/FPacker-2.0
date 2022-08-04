using FPacker.P3D.IO;

namespace FPacker.P3D.Math; 

public class Point : Vector3P {
    public PointFlags PointFlags { get; private set; }

    public Point(Vector3P pos, PointFlags flags) : base(pos.X, pos.Y, pos.Z) => PointFlags = flags;

    public Point(P3DBinaryReader input) : base(input) => PointFlags = (PointFlags) input.ReadUInt32();

    public new void WriteObject(P3DBinaryWriter output) {
        base.WriteObject(output);
        output.Write((uint)PointFlags);
    }

}