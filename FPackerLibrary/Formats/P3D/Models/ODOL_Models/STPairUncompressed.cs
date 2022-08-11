using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class STPairUncompressed : STPair, IDeserializable {
    public void ReadObject(P3DBinaryReader input) {
        S.ReadObject(input);
        T.ReadObject(input);
    }

    public void WriteObject(P3DBinaryWriter output) {
        S.WriteObject(output);
        T.WriteObject(output);
    }
}