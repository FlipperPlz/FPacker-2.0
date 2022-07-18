using FPacker.P3D.IO;

namespace FPacker.P3D.Models.MLOD.Tagg; 

public class SharpEdgesTagg : Tagg {
    public uint[,] PointIndices;
    
    public void Read(P3DBinaryReader input) {
        var num = DataSize / 8U;
        PointIndices = new uint[(int)((IntPtr)num), 2];
        var num2 = 0;
        while (num2 < num) {
            PointIndices[num2, 0] = input.ReadUInt32();
            PointIndices[num2, 1] = input.ReadUInt32();
            num2++;
        }
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(base.Name);
        output.Write(DataSize);
        var num = DataSize / 8U;
        var num2 = 0;
        while (num2 < num) {
            output.Write(PointIndices[num2, 0]);
            output.Write(PointIndices[num2, 1]);
            num2++;
        }
    }
}