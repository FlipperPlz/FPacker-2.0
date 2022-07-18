using FPacker.P3D.IO;

namespace FPacker.P3D.Models.MLOD.Tagg; 

public class MassTagg : Tagg {
    public float[] Mass;

    public void Read(P3DBinaryReader input) {
        var num = base.DataSize / 4U;
        this.Mass = new float[num];
        var num2 = 0;
        while ((long)num2 < (long)((ulong)num)) {
            this.Mass[num2] = input.ReadSingle();
            num2++;
        }
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(base.Name);
        output.Write(base.DataSize);
        var num = base.DataSize / 4U;
        var num2 = 0;
        while ((long)num2 < (long)((ulong)num)) {
            output.Write(this.Mass[num2]);
            num2++;
        }
    }
}