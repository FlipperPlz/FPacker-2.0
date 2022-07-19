
using FPacker.Formats.RAP.Models.Parse;
using FPacker.P3D.IO;
using FPacker.P3D.Models.MLOD;
using FPacker.P3D.Models.ODOL;

namespace FPacker;

public class Program {
    public static void Main(string[] arguments) {
        //var p3d = P3D.Models.P3D.GetInstance(
        //    @"C:\Users\dev\Desktop\FPacker 2.0\testing\tf\JSmqVYaVFTGEECXBujePZavRN.p3d") as MLOD;
        //Console.WriteLine();
        //new AddonPacker(@"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGuns\RevGuns", @"C:\Users\dev\Desktop\PlasmaMod.pbo");
        var v = new RapBinaryParser(
            @"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\config-9.bin");
    }
}