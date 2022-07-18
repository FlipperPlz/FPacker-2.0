using FPacker.P3D.IO;

namespace FPacker.P3D.Math; 

public class Vertex {
    public int PointIndex { get; private set; }
    public int NormalIndex { get; private set; }
    public float U { get; private set; }
    public float V { get; private set; }
    
    public Vertex(P3DBinaryReader input) => Read(input);
    
    public Vertex(int point, int normal, float u, float v) {
        PointIndex = point;
        NormalIndex = normal;
        U = u;
        V = v;
    }
    
    public void Read(P3DBinaryReader input) {
        PointIndex = input.ReadInt32();
        NormalIndex = input.ReadInt32();
        U = input.ReadSingle();
        V = input.ReadSingle();
    }
    
    public void Write(P3DBinaryWriter output)
    {
        output.Write(PointIndex);
        output.Write(NormalIndex);
        output.Write(U);
        output.Write(V);
    }
}