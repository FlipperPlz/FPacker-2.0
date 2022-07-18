using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public class Polygon : IDeserializable{
    public VertexIndex[] VertexIndices { get; private set; }
    
    public void ReadObject(P3DBinaryReader input) {
        var version = input.Version;
        var b = input.ReadByte();
        this.VertexIndices = new VertexIndex[b];
        for (var i = 0; i < (int)b; i++) this.VertexIndices[i] = input.ReadVertexIndex();
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.Write((byte) VertexIndices.Length);
        foreach (var vertexIndex in VertexIndices) output.WriteVertexIndex(vertexIndex);
    }
}