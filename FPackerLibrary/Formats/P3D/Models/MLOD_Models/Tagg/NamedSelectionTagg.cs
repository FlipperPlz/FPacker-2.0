using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.MLOD.Tagg; 

public class NamedSelectionTagg : Tagg {
    public byte[] Points;
    public byte[] Faces;
    
    public void Read(P3DBinaryReader input, int nPoints, int nFaces) {
        Points = new byte[nPoints];
        for (var i = 0; i < nPoints; i++) Points[i] = input.ReadByte();
        Faces = new byte[nFaces];
        for (var j = 0; j < nFaces; j++) Faces[j] = input.ReadByte();
    }

    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(Name);
        output.Write(DataSize);
        foreach (var t in Points) output.Write(t);
        foreach (var t in Faces) output.Write(t);
    }
}