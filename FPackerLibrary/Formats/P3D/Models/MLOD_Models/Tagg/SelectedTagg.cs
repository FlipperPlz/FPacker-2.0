using FPacker.P3D.IO;

namespace FPacker.P3D.Models.MLOD.Tagg; 

public class SelectedTagg : Tagg {
    public byte[] WeightedPoints;
    public byte[] Faces;
    
    public void Read(P3DBinaryReader input, int nPoints, int nFaces) {
        WeightedPoints = new byte[nPoints];
        for (var i = 0; i < nPoints; i++) WeightedPoints[i] = input.ReadByte();
        Faces = new byte[nFaces];
        for (var j = 0; j < nFaces; j++) Faces[j] = input.ReadByte();
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(base.Name);
        output.Write(base.DataSize);
        foreach (var t in this.WeightedPoints) output.Write(t);
        foreach (var t in this.Faces) output.Write(t);
    }
}