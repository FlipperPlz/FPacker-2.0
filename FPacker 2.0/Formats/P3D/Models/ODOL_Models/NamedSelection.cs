using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public class NamedSelection : IDeserializable {  
    public string Name { get; private set; }
    public bool IsSectional { get; private set; }
    public VertexIndex[] SelectedFaces { get; private set; }
    public int[] Sections { get; private set; }
    public byte[] SelectedVerticesWeights { get; private set; }
    public VertexIndex[] SelectedVertices { get; private set; }

    private int unknownInt;
    
    public void ReadObject(P3DBinaryReader input)
    {
        Name = input.ReadAsciiZ();
        SelectedFaces = input.ReadCompressedVertexIndexArray();
        unknownInt = input.ReadInt32();
        IsSectional = input.ReadBoolean();
        Sections = input.ReadCompressedIntArray();
        SelectedVertices = input.ReadCompressedVertexIndexArray();
        var expectedSize = input.ReadInt32();
        SelectedVerticesWeights = input.ReadCompressed((uint)expectedSize);
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.WriteAsciiZ(Name);
        output.WriteCompressedVertexIndexArray(SelectedFaces);
        output.WriteInt32(unknownInt);
        output.Write(IsSectional);
        output.WriteCompressedIntArray(Sections);
        output.WriteCompressedVertexIndexArray(SelectedVertices);
        
        output.WriteCompressed(SelectedVerticesWeights);
    }
}