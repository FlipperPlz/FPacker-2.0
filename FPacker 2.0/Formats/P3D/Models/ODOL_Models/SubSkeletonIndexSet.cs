using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL; 

public class SubSkeletonIndexSet : IDeserializable {
    private int[] _subSkeletons;

    public void ReadObject(P3DBinaryReader input) => _subSkeletons = input.ReadIntArray();

    public void WriteObject(P3DBinaryWriter output) => output.WriteArray(_subSkeletons, static (output, i) => output.WriteInt32(i));
}