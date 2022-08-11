using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.MLOD.Tagg; 

public class UVSetTagg : Tagg {
    public uint UVSetNr;
    public float[][,] FaceUVs;
    
    public void Read(P3DBinaryReader input, Face[] faces) {
        UVSetNr = input.ReadUInt32();
        FaceUVs = new float[faces.Length][,];
        for (var i = 0; i < faces.Length; i++) {
            FaceUVs[i] = new float[faces[i].NumberOfVertices, 2];
            for (var j = 0; j < faces[i].NumberOfVertices; j++) {
                FaceUVs[i][j, 0] = input.ReadSingle();
                FaceUVs[i][j, 1] = input.ReadSingle();
            }
        }
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(base.Name);
        output.Write(base.DataSize);
        output.Write(UVSetNr);
        foreach (var t in FaceUVs) {
            for (var j = 0; j < t.Length / 2; j++) {
                output.Write(t[j, 0]);
                output.Write(t[j, 1]);
            }
        }
    }
}