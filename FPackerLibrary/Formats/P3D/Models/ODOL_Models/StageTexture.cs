using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class StageTexture {
    
    public TextureFilterType TextureFilter;
    public string Texture;
    public uint StageId;
    public bool UseWorldEnvMap;
    
    public void Read(P3DBinaryReader input, uint matVersion) {
        if (matVersion >= 5U) TextureFilter = (TextureFilterType)input.ReadUInt32();
        Texture = input.ReadAsciiZ();
        if (matVersion >= 8U) StageId = input.ReadUInt32();
        if (matVersion >= 11U) UseWorldEnvMap = input.ReadBoolean();
    }
    
    public enum TextureFilterType {
        Point,
        Linear,
        TriLinear,
        Anisotropic
    }

    public void Write(P3DBinaryWriter output, uint version) {
        if (version >= 5U) output.WriteUInt32((uint) TextureFilter);
        if (version >= 8U) output.WriteUInt32(StageId);
        if (version >= 11U) output.Write(UseWorldEnvMap);
    }
}