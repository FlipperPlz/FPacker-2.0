using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public class STPairCompressed  : STPair, IDeserializable {
    public void ReadObject(P3DBinaryReader input) {
        S.ReadCompressed(input);
        T.ReadCompressed(input);
    }

    public void WriteObject(P3DBinaryWriter output) {
        S.WriteCompressed(output);
        T.WriteCompressed(output);
    }
}