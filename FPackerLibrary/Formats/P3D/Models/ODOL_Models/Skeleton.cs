using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class Skeleton {
    public string Name { get; }
    public bool Discrete { get; }
    public string[] Bones { get; }
    public string PivotsNameObsolete { get; }
    
    private int _boneCount { get; }

    
    public Skeleton(P3DBinaryReader input) {
        Name = input.ReadAsciiZ();
        if (Name == "") return;
        if (input.Version >= 23) Discrete = input.ReadBoolean();
        _boneCount = input.ReadInt32();
        Bones = new string[_boneCount * 2];
        for (var i = 0; i < _boneCount; i++) {
            Bones[i * 2] = input.ReadAsciiZ();
            Bones[i * 2 + 1] = input.ReadAsciiZ();
        }
        if (input.Version > 40) this.PivotsNameObsolete = input.ReadAsciiZ();
    }

    public void Write(P3DBinaryWriter output) {
        output.WriteAsciiZ(Name);
        if(Name == string.Empty) return;
        if (output.Version >= 23U) output.Write(Discrete);
        output.WriteInt32(_boneCount);
        for (var i = 0; i < _boneCount; i++) {
            output.WriteAsciiZ(Bones[i * 2]);
            output.WriteAsciiZ(Bones[i * 2 + 1]);
        }
        if (output.Version > 40U) output.WriteAsciiZ(PivotsNameObsolete);
    }
}