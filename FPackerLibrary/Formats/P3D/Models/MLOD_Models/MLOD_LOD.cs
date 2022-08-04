using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;
using FPackerLibrary.P3D.Models.MLOD.Tagg;

namespace FPackerLibrary.P3D.Models.MLOD; 

public class MLOD_LOD : P3D_LOD {
    private uint _unk1;
    private Point[] _points;
    private Vector3P[] _normals;
    public Face[] Faces;
    public List<Tagg.Tagg> Taggs;
    public override Vector3P[] Points {
        get => _points;
        set => _points = value as Point[];
    }

    public override Vector3P[] Normals {
        get => this._normals;
        set => _normals = value;
    }

    public override string[] Textures => (from f in this.Faces select f.Texture).Distinct().ToArray();

    public override string[] MaterialNames => (from f in this.Faces select f.Material).Distinct().ToArray();

    public MLOD_LOD() { }

    public MLOD_LOD(float resolution) => this.resolution = resolution;
    

    private Tagg.Tagg ReadTagg(P3DBinaryReader input) {
        Tagg.Tagg tagg = new MassTagg();
        if (!input.ReadBoolean()) throw new Exception("Deactivated Tagg?");

        tagg.Name = input.ReadAsciiZ();
        tagg.DataSize = input.ReadUInt32();
        switch (tagg.Name) {
            case "#SharpEdges#": {
                var sharpEdgesTagg = new SharpEdgesTagg {
                    Name = "#SharpEdges#",
                    DataSize = tagg.DataSize
                };
                sharpEdgesTagg.Read(input);
                return sharpEdgesTagg;
            }
            case "#Property#": {
                var propertyTagg = new PropertyTagg {
                    Name = "#Property#",
                    DataSize = tagg.DataSize
                };
                propertyTagg.Read(input);
                return propertyTagg;
            }
            case "#Mass#": {
                var massTagg = new MassTagg {
                    Name = "#Mass#",
                    DataSize = tagg.DataSize
                };
                massTagg.Read(input);
                return massTagg;
            }
            case "#UVSet#": {
                var uVSetTagg = new UVSetTagg {
                    Name = "#UVSet#",
                    DataSize = tagg.DataSize
                };
                uVSetTagg.Read(input, Faces);
                return uVSetTagg;
            }
            case "#Lock#": {
                var lockTagg = new LockTagg {
                    Name = "#Lock#",
                    DataSize = tagg.DataSize
                };
                lockTagg.Read(input, Points.Length, Faces.Length);
                return lockTagg;
            }
            case "#Selected#": {
                var selectedTagg = new SelectedTagg {
                    Name = "#Selected#",
                    DataSize = tagg.DataSize
                };
                selectedTagg.Read(input, Points.Length, Faces.Length);
                return selectedTagg;
            }
            case "#Animation#": {
                var animationTagg = new AnimationTagg {
                    Name = "#Animation#",
                    DataSize = tagg.DataSize
                };
                animationTagg.Read(input);
                return animationTagg;
            }
            case "#EndOfFile#":
                return tagg;
            default: {
                var namedSelectionTagg = new NamedSelectionTagg {
                    Name = tagg.Name,
                    DataSize = tagg.DataSize
                };
                namedSelectionTagg.Read(input, Points.Length, Faces.Length);
                return namedSelectionTagg;
            }
        }
    }

    private void WriteTagg(P3DBinaryWriter output, Tagg.Tagg tagg) {
        switch (tagg.Name) {
            case "#SharpEdges#":
                ((SharpEdgesTagg) tagg).Write(output);
                break;
            case "#Property#":
                ((PropertyTagg) tagg).Write(output);
                break;
            case "#Mass#":
                ((MassTagg) tagg).Write(output);
                break;
            case "#UVSet#":
                ((UVSetTagg) tagg).Write(output);
                break;
            case "#Lock#":
                ((LockTagg) tagg).Write(output);
                break;
            case "#Selected#":
                ((SelectedTagg) tagg).Write(output);
                break;
            case "#Animation#":
                ((AnimationTagg) tagg).Write(output);
                break;
            default:
                ((NamedSelectionTagg) tagg).Write(output);
                break;
            case "#EndOfFile#":
                break;
        }
    }

    public void Read(P3DBinaryReader input) {
        if (input.ReadAscii(4) != "P3DM") throw new Exception("Only P3DM LODs are supported");
        if (input.ReadUInt32() != 28 || input.ReadUInt32() != 256) throw new Exception("Unknown P3DM version");

        var num = input.ReadUInt32();
        var num2 = input.ReadUInt32();
        var num3 = input.ReadUInt32();
        _unk1 = input.ReadUInt32();
        _points = new Point[num];
        _normals = new Vector3P[num2];
        Faces = new Face[num3];
        for (var i = 0; i < num; i++) _points[i] = new Point(input);

        for (var j = 0; j < num2; j++) _normals[j] = new Vector3P(input);

        for (var k = 0; k < num3; k++) Faces[k] = new Face(input);

        if (input.ReadAscii(4) != "TAGG") throw new Exception("TAGG expected");

        Taggs = new List<Tagg.Tagg>();
        Tagg.Tagg tagg;
        do {
            tagg = ReadTagg(input);
            if (tagg.Name != "#EndOfFile#") Taggs.Add(tagg);
        } while (tagg.Name != "#EndOfFile#");

        resolution = input.ReadSingle();
    }

    public void Write(P3DBinaryWriter output) {
        var num = _points.Length;
        var num2 = _normals.Length;
        var num3 = Faces.Length;
        output.WriteAscii("P3DM", 4u);
        output.Write(28);
        output.Write(256);
        output.Write(num);
        output.Write(num2);
        output.Write(num3);
        output.Write(_unk1);
        for (var i = 0; i < num; i++) _points[i].WriteObject(output);
        for (var j = 0; j < num2; j++) _normals[j].WriteObject(output);
        for (var k = 0; k < num3; k++) Faces[k].WriteObject(output);

        output.WriteAscii("TAGG", 4u);
        foreach (var tagg in Taggs) WriteTagg(output, tagg);

        output.Write(value: true);
        output.WriteAsciiZ("#EndOfFile#");
        output.Write(0);
        output.Write(resolution);
    }

    public float GetHeight() {
        var num = this.Points.Length;
        if (num <= 1L) return 0f;

        var num2 = float.MaxValue;
        var num3 = float.MinValue;
        for (var i = 0; i < num; i++) {
            var y = this._points[i].Y;
            if (y > num3) num3 = y;
            if (y < num2) num2 = y;
        }
        return num3 - num2;
    }

}