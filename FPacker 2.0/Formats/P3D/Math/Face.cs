using FPacker.P3D.IO;

namespace FPacker.P3D.Math; 

public class Face {
    public int NumberOfVertices { get; private set; }
    public Vertex[] Vertices { get; private set; }
    public FaceFlags Flags { get; private set; }
    public string Texture { get; set; }
    public string Material { get; set; }

    public Face(int nVerts, Vertex[] verts, FaceFlags flags, string texture, string material) {
        NumberOfVertices = nVerts;
        Vertices = verts;
        Flags = flags;
        Texture = texture;
        Material = material;
    }
    
    public Face(P3DBinaryReader input) => Read(input);
    
    public void Read(P3DBinaryReader input) {
        NumberOfVertices = input.ReadInt32();
        Vertices = new Vertex[4];
        for (var i = 0; i < 4; i++) Vertices[i] = new Vertex(input);
        Flags = (FaceFlags)input.ReadInt32();
        Texture = input.ReadAsciiZ();
        Material = input.ReadAsciiZ();
    }
    
    public void WriteObject(P3DBinaryWriter output) {
        output.Write(NumberOfVertices);
        for (var i = 0; i < 4; i++) {
            if (i < Vertices.Length && Vertices[i] != null) Vertices[i].Write(output);
            else {
                output.Write(0);
                output.Write(0);
                output.Write(0);
                output.Write(0);
            }
        }
        output.Write((int)Flags);
        output.WriteAsciiZ(Texture);
        output.WriteAsciiZ(Material);
    }
    
}