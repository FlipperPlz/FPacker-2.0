using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class Proxy : IDeserializable {
    
    public string ProxyModel;
    public Matrix4P Transformation;
    public int SequenceId;
    public int NamedSelectionIndex;
    public int BoneIndex;
    public int SectionIndex;
    
    public void ReadObject(P3DBinaryReader input) {
        this.ProxyModel = input.ReadAsciiZ();
        this.Transformation = new Matrix4P(input);
        this.SequenceId = input.ReadInt32();
        this.NamedSelectionIndex = input.ReadInt32();
        this.BoneIndex = input.ReadInt32();
        if (input.Version >= 40) this.SectionIndex = input.ReadInt32();
    }

    public void WriteObject(P3DBinaryWriter writer) {
        writer.WriteAsciiZ(ProxyModel);
        Transformation.WriteObject(writer);
        writer.WriteInt32(SequenceId);
        writer.WriteInt32(NamedSelectionIndex);
        writer.WriteInt32(BoneIndex);
        if (writer.Version >= 40U) writer.WriteInt32(SectionIndex);

    }
}