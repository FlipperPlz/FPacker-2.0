using FPacker.Antlr.Enforce;
using FPacker.P3D.IO;

namespace FPacker.P3D.Models; 

public abstract class P3D {
    public uint Version { get; protected set; }
    public abstract P3D_LOD[] LODs { get; }
    public abstract float Mass { get; }

    public static P3D GetInstance(string fileName) {
        System.IO.Stream stream = File.OpenRead(fileName);
        var type = new P3DBinaryReader( stream ).ReadAscii( 4 );
        stream.Close();
        return type switch {
            "ODOL" => new ODOL.ODOL(fileName),
            "MLOD" => new MLOD.MLOD(fileName),
            _ => throw new FormatException()
        };
    }
    
    public virtual P3D_LOD GetLOD(float resolution) => LODs.FirstOrDefault((P3D_LOD lod) => lod.Resolution == resolution);
}