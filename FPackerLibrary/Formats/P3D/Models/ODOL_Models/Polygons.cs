using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class Polygons {
    public Polygon[] Faces { get; private set; }
    public uint unknown1;
    public ushort unknown2;


    public Polygons(P3DBinaryReader input) {
        var num = input.ReadUInt32();
        unknown1 = input.ReadUInt32();
        unknown2 = input.ReadUInt16();
        this.Faces = new Polygon[num];
        var num2 = 0;
        while (num2 < num) {
            this.Faces[num2] = new Polygon();
            this.Faces[num2].ReadObject(input);
            num2++;
        }
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.WriteUInt32((uint) Faces.Length);
        output.WriteUInt32(unknown1);
        output.Write(unknown2);
        var num2 = 0;
        while (num2 < Faces.Length) {
            this.Faces[num2].WriteObject(output);
            num2++;
        }
    }
}