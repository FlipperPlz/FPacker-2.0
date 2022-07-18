
using FPacker.P3D.IO;

namespace FPacker.P3D.Models.MLOD.Tagg; 

public class LockTagg : Tagg{
    public bool[] LockedPoints;
    public bool[] LockedFaces;
    
    public void Read(P3DBinaryReader input, int nPoints, int nFaces) {
        this.LockedPoints = new bool[nPoints];
        for (var i = 0; i < nPoints; i++) this.LockedPoints[i] = input.ReadBoolean();
        this.LockedFaces = new bool[nFaces];
        for (var j = 0; j < nFaces; j++) this.LockedFaces[j] = input.ReadBoolean();
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(base.Name);
        output.Write(base.DataSize);
        foreach (var t in this.LockedPoints) output.Write(t);
        foreach (var t in this.LockedFaces) output.Write(t);
    }
}