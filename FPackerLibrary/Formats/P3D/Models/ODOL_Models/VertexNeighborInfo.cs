using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public class VertexNeighborInfo : IDeserializable {
    public ushort PosA { get; private set; }
    public AnimationRTWeight RtwA { get; private set; }
    public ushort PosB { get; private set; }
    public AnimationRTWeight RtwB { get; private set; }

    public byte[] unknownBytes1;
    public byte[] unknownBytes2;

    public void ReadObject(P3DBinaryReader input)
    {
        this.PosA = input.ReadUInt16();
        unknownBytes1 = input.ReadBytes(2);
        this.RtwA = new AnimationRTWeight();
        this.RtwA.ReadObject(input);
        this.PosB = input.ReadUInt16();
        unknownBytes2 = input.ReadBytes(2);
        this.RtwB = new AnimationRTWeight();
        this.RtwB.ReadObject(input);
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.Write(PosA);
        output.Write(unknownBytes1);
        RtwA.WriteObject(output);
        output.Write(PosB);
        output.Write(unknownBytes2);
        RtwB.WriteObject(output);



    }
}