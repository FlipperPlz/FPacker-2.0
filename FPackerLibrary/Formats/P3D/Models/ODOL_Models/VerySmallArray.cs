using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class VerySmallArray : IDeserializable {
    protected int NSmall;
    protected byte[] SmallSpace;
    
    public void ReadObject(P3DBinaryReader input) {
        this.NSmall = input.ReadInt32();
        this.SmallSpace = input.ReadBytes(8);
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.WriteInt32(NSmall);
        output.Write(SmallSpace);
    }
}