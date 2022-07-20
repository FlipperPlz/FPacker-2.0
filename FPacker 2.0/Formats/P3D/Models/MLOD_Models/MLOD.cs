using FPacker.P3D.IO;

namespace FPacker.P3D.Models.MLOD; 

public class MLOD : P3D {
    public override P3D_LOD[] LODs => this.lods;
    public override float Mass => throw new NotImplementedException();
    private MLOD_LOD[] lods;

    public MLOD(string fileName) {
        using var binaryReaderEx = new P3DBinaryReader(File.OpenRead(fileName));
        Read(binaryReaderEx);
    }
    
    public MLOD(Stream stream) => this.Read(new P3DBinaryReader(stream));
    
    public MLOD(MLOD_LOD[] lods) => this.lods = lods;
    
    private void Read(P3DBinaryReader input) {
        if (input.ReadAscii(4) != "MLOD") throw new Exception("MLOD signature expected");
        Version = input.ReadUInt32();
        if (Version != 257U) throw new Exception("Unknown MLOD version");
        var num = input.ReadUInt32();
        lods = new MLOD_LOD[num];
        var num2 = 0;
        while (num2 < num){
            lods[num2] = new MLOD_LOD();
            lods[num2].Read(input);
            num2++;
        }
    }
    
    private void Write(P3DBinaryWriter output) {
        output.WriteAscii("MLOD", 4U);
        output.Write(257);
        output.Write(lods.Length);
        foreach (var lod in lods) lod.Write(output);
    }
    
    public bool WriteToFile(string file, bool allowOverwriting = false) {
        try {
            var mode = (!allowOverwriting) ? FileMode.CreateNew : FileMode.Create;
            var BinaryWriterEx = new P3DBinaryWriter(new FileStream(file, mode));
            this.Write(BinaryWriterEx);
            BinaryWriterEx.Close();
        } catch (IOException ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
        return true;
    }

    public MemoryStream WriteToMemory() {
        var memoryStream = new MemoryStream(100000);
        var BinaryWriterEx = new P3DBinaryWriter(memoryStream);
        Write(BinaryWriterEx);
        BinaryWriterEx.Position = 0L;
        return memoryStream;
    }
    
    public void WriteToStream(Stream stream) {
        var output = new P3DBinaryWriter(stream);
        Write(output);
    }
}