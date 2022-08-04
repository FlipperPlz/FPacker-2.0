using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.MLOD.Tagg; 

public class PropertyTagg : Tagg {
    public string PropertyName;
    public string PropertyValue;
    
    public void Read(P3DBinaryReader input) {
        PropertyName = input.ReadAscii(64);
        PropertyValue = input.ReadAscii(64);
    }
    
    public void Write(P3DBinaryWriter output) {
        output.Write(true);
        output.WriteAsciiZ(Name);
        output.Write(DataSize);
        output.WriteAscii(PropertyName, 64U);
        output.WriteAscii(PropertyValue, 64U);
    }
    
}