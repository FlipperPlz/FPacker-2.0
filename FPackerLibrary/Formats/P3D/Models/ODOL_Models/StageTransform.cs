using FPacker.P3D.IO;
using FPacker.P3D.Math;

namespace FPacker.P3D.Models.ODOL; 

public class StageTransform {
    public UVSource UvSource;
    public Matrix4P Transformation;
    
    public StageTransform(P3DBinaryReader input) {
        UvSource = (UVSource)input.ReadUInt32();
        Transformation = new Matrix4P(input);
    }


    public enum UVSource {
        UVNone,
        UVTex,
        UVTexWaterAnim,
        UVPos,
        UVNorm,
        UVTex1,
        UVWorldPos,
        UVWorldNorm,
        UVTexShoreAnim,
        NUVSource
    }

    public void Write(P3DBinaryWriter output) {
        output.WriteUInt32((uint) UvSource);
        Transformation.WriteObject(output);
    }
}