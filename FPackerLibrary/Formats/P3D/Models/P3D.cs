using FPackerLibrary.Antlr.Enforce;
using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models; 

public abstract class P3D {
    public uint Version { get; protected set; }
    public abstract P3D_LOD[] LODs { get; }
    public abstract float Mass { get; }

    public static P3D GetInstance(string fileName) => GetInstance(File.OpenRead(fileName));

    public virtual P3D_LOD GetLOD(float resolution) => LODs.FirstOrDefault((P3D_LOD lod) => lod.Resolution == resolution);

    public static P3D GetInstance(Stream stream) { 
        var type = new P3DBinaryReader( stream ).ReadAscii( 4 );
        stream.Position -= 4;
        
        return type switch {
            "ODOL" => new ODOL.ODOL(stream),
            "MLOD" => new MLOD.MLOD(stream),
            _ => throw new FormatException()
        };
    }
}