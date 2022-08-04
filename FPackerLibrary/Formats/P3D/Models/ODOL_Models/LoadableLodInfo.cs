using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Models.ODOL; 

public class LoadableLodInfo : IDeserializable {
    
    private int _nFaces;
    private uint _color;
    private int _special;
    private uint _orHints;
    private bool _hasSkeleton;
    private int _nVertices;
    private float _faceArea;
    
    public void ReadObject(P3DBinaryReader input) {
        _nFaces = input.ReadInt32();
        _color = input.ReadUInt32();
        _special = input.ReadInt32();
        _orHints = input.ReadUInt32();
        var version = input.Version;
        if (version >= 39) _hasSkeleton = input.ReadBoolean();
        if (version < 51) return;
        _nVertices = input.ReadInt32();
        _faceArea = input.ReadSingle();
    }

    public void WriteObject(P3DBinaryWriter output) {
        output.WriteInt32(_nFaces);
        output.WriteUInt32(_color);
        output.WriteInt32(_special);
        output.WriteUInt32(_orHints);
        if (output.Version >= 39U) output.Write(_hasSkeleton);
        if (output.Version < 39U) return;
        output.WriteInt32(_nVertices);
        output.WriteSingle(_faceArea);
    }
}