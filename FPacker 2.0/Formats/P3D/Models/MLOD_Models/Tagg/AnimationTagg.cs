using FPacker.P3D.IO;
using FPacker.P3D.Math;

namespace FPacker.P3D.Models.MLOD.Tagg; 

public class AnimationTagg : Tagg {
    public float FrameTime;
    public Vector3P[] FramePoints;
    
    public void Read(P3DBinaryReader input) {
        var num = (DataSize - 4U) / 12U;
        FrameTime = input.ReadSingle(); 
        FramePoints = new Vector3P[num];
        var num2 = 0;
        while (num2 < num) {
            FramePoints[num2] = new Vector3P(input);
            num2++;
        }
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(Name);
        output.Write(DataSize);
        output.Write(this.FrameTime);
        foreach (var t in FramePoints) t.WriteObject(output);
    }
}