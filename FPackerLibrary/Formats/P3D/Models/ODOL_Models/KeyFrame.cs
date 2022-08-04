using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class Keyframe : IDeserializable {
    public float Time;
    public Vector3P[] Points;
    
    public void ReadObject(P3DBinaryReader input) {
        this.Time = input.ReadSingle();
        var num = input.ReadUInt32();
        this.Points = new Vector3P[num];
        var num2 = 0;
        while ((long)num2 < (long)((ulong)num)) {
            this.Points[num2] = new Vector3P(input);
            num2++;
        }
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.WriteSingle(Time);
        output.WriteUInt32((uint) Points.Length);
        var num2 = 0;

        while (num2 < Points.Length) {
            Points[num2].WriteObject(output);
            num2++;
        }
    }
}