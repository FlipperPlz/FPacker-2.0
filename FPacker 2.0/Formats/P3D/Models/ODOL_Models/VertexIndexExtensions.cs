using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public static class VertexIndexExtensions {
    public static VertexIndex ReadVertexIndex(this P3DBinaryReader input) => input.Version >= 69 ? input.ReadInt32() : input.ReadUInt16();

    public static void WriteVertexIndex(this P3DBinaryWriter output, VertexIndex index) {
        if (output.Version >= 69U) {
            output.Write((int) index);
        }
        else output.Write((ushort) index);
    }

    // Token: 0x06000167 RID: 359 RVA: 0x00007194 File Offset: 0x00005394
    public static VertexIndex[] ReadCompressedVertexIndexArray(this P3DBinaryReader input) =>
        input.Version >= 69 
        ? input.ReadCompressedArray<VertexIndex>(static i => i.ReadInt32(), 4) 
        : input.ReadCompressedArray<VertexIndex>(static i => i.ReadUInt16(), 2);

    public static void WriteCompressedVertexIndexArray(this P3DBinaryWriter output, VertexIndex[] vertexIndices) {
        if (output.Version >= 69)
            output.WriteCompressedArray<VertexIndex>(vertexIndices,
                static (writer, index) => writer.WriteInt32(index), 4);
        else
            output.WriteCompressedArray<VertexIndex>(vertexIndices,
                static (writer, index) => writer.Write((ushort) index), 2);

    }
        
}