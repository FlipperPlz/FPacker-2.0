using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.ODOL;

public class LOD : P3D_LOD, IComparable<LOD> {
	private uint _odolVersion;
	private SubSkeletonIndexSet[] _skeletonToSubSkeleton;
	private uint _vertexCount;
	private float _faceArea;
	private ClipFlags[] _clipOldFormat;
	private ClipFlags[] _clip;
	private ClipFlags _orHints;
	private ClipFlags _andHints;
	private Vector3P _bMin;
	private Vector3P _bMax;
	private Vector3P _bCenter;
	private float _bRadius;
	private string[] _textures;
	private VertexIndex[] _pointToVertex;
	private VertexIndex[] _vertexToPoint;
	private Polygons _polygons;
	private uint _nNamedProperties;
	private int _colorTop;
	private int _color;
	private int _special;
	private bool _vertexBoneRefIsSimple;
	private uint _sizeOfRestData;
	private uint _nUvSets;
	private Vector3P[] _normals;
	private STPair[] _stCoords;
	public NamedSelection[] NamedSelections { get; private set; }
	public override string[] MaterialNames => (from m in Materials select m.MaterialName).ToArray();

	public EmbeddedMaterial[] Materials { get; private set; }
	public int VertexCount => Vertices.Length;
	public int SectionCount => Sections.Length;
	public int TextureCount => _textures.Length;
	public int PolygonCount => _polygons.Faces.Length;
	public int MaterialCount => Materials.Length;
	public AnimationRTWeight[] VertexBoneRef { get; private set; }
	public VertexNeighborInfo[] NeighborBoneRef { get; private set; }
	public ClipFlags[] ClipFlags => _odolVersion < 50U ? _clipOldFormat : _clip;
	public Vector3P[] Vertices { get; private set; }
	public override Vector3P[] Normals { get => _normals; set => _normals = value; }

	public Section[] Sections { get; private set; }
	public UVSet[] UVSets { get; private set; }
	public Polygon[] Faces => _polygons.Faces;
	public string[,] NamedProperties { get; private set; }
	public Keyframe[] Frames { get; private set; }
	public int[] SubSkeletonsToSkeleton { get; private set; }
	public Proxy[] Proxies { get; private set; }
	public override Vector3P[] Points { get => Vertices; set => Vertices = value; }

	public override string[] Textures => _textures;

	public uint unknownInt;
	public byte unknownByte;

	public void Read(P3DBinaryReader input, float resolution) {
		_odolVersion = (uint) input.Version;
		this.resolution = resolution;
		Proxies = input.ReadArray<Proxy>();
		SubSkeletonsToSkeleton = input.ReadIntArray();
		_skeletonToSubSkeleton = input.ReadArray<SubSkeletonIndexSet>();
		if (_odolVersion >= 50) _vertexCount = input.ReadUInt32();
		else {
			var array = input.ReadCondensedIntArray();
			_clipOldFormat = Array.ConvertAll(array, (int item) => (ClipFlags) item);
		}

		if (_odolVersion >= 51) _faceArea = input.ReadSingle();
		_orHints = (ClipFlags) input.ReadInt32();
		_andHints = (ClipFlags) input.ReadInt32();
		_bMin = new Vector3P(input);
		_bMax = new Vector3P(input);
		_bCenter = new Vector3P(input);
		_bRadius = input.ReadSingle();
		_textures = input.ReadStringArray();
		Materials = input.ReadArray<EmbeddedMaterial>();
		_pointToVertex = input.ReadCompressedVertexIndexArray();
		_vertexToPoint = input.ReadCompressedVertexIndexArray();
		_polygons = new Polygons(input);
		Sections = input.ReadArray<Section>();
		NamedSelections = input.ReadArray<NamedSelection>();
		_nNamedProperties = input.ReadUInt32();
		NamedProperties = new string[_nNamedProperties, 2];
		for (var i = 0; i < _nNamedProperties; i++) {
			NamedProperties[i, 0] = input.ReadAsciiZ();
			NamedProperties[i, 1] = input.ReadAsciiZ();
		}

		Frames = input.ReadArray<Keyframe>();
		_colorTop = input.ReadInt32();
		_color = input.ReadInt32();
		_special = input.ReadInt32();
		_vertexBoneRefIsSimple = input.ReadBoolean();
		_sizeOfRestData = input.ReadUInt32();
		if (_odolVersion >= 50) {
			var array2 = input.ReadCondensedIntArray();
			_clip = Array.ConvertAll(array2, (int item) => (ClipFlags) item);
		}

		var uVSet = new UVSet();
		uVSet.Read(input, _odolVersion);
		_nUvSets = input.ReadUInt32();
		UVSets = new UVSet[_nUvSets];
		UVSets[0] = uVSet;
		for (var j = 1; j < _nUvSets; j++) {
			UVSets[j] = new UVSet();
			UVSets[j].Read(input, _odolVersion);
		}

		Vertices = input.ReadCompressedObjectArray<Vector3P>(12);
		if (_odolVersion >= 45) {
			var array3 = input.ReadCondensedObjectArray<Vector3PCompressed>(4);
			_normals = Array.ConvertAll(array3,
				(Converter<Vector3PCompressed, Vector3P>) (item => item));
		} else _normals = input.ReadCondensedObjectArray<Vector3P>(12);
		_stCoords = (STPair[]) (_odolVersion >= 45
			? (Array) input.ReadCompressedObjectArray<STPairCompressed>(8)
			: (Array) input.ReadCompressedObjectArray<STPairUncompressed>(24));
		VertexBoneRef = input.ReadCompressedObjectArray<AnimationRTWeight>(12);
		NeighborBoneRef = input.ReadCompressedObjectArray<VertexNeighborInfo>(32);
		if (_odolVersion >= 67) unknownInt = input.ReadUInt32();
		if (_odolVersion >= 68) unknownByte = input.ReadByte();
	}

	public void Write(P3DBinaryWriter output, float f) {
		output.WriteArray(Proxies, static (writer, proxy) => proxy.WriteObject(writer));
		output.WriteArray(SubSkeletonsToSkeleton, static (writer, i) => writer.WriteInt32(i));
		output.WriteArray(_skeletonToSubSkeleton, static (writer, set) => set.WriteObject(writer) );
		if (_odolVersion >= 50U) output.WriteUInt32(_vertexCount);
		else {
			var floats = _clipOldFormat.Select(static c => (float) c).ToList();
			output.WriteCondensedIntArray(floats.ToArray());
		}
		if (_odolVersion >= 51) output.WriteSingle(_faceArea);
		output.WriteInt32((int) _orHints);
		output.WriteInt32((int) _andHints);
		_bMin.WriteObject(output);
		_bMax.WriteObject(output);
		_bCenter.WriteObject(output);
		output.WriteSingle(_bRadius);
		output.WriteArray(_textures, static (writer, s) => writer.WriteAsciiZ(s));
		output.WriteArray(Materials, static (writer, material) => material.WriteObject(writer));
		output.WriteCompressedVertexIndexArray(_pointToVertex);
		output.WriteCompressedVertexIndexArray(_vertexToPoint);
		_polygons.WriteObject(output);
		output.WriteArray(Sections, static (writer, section) => section.WriteObject(writer));
		output.WriteArray(NamedSelections, static (writer, section) => section.WriteObject(writer));
		output.WriteUInt32(_nNamedProperties);
		for (var i = 0; i < _nNamedProperties; i++) {
			output.WriteAsciiZ(NamedProperties[i, 0]);
			output.WriteAsciiZ(NamedProperties[i, 1]);
		}
		output.WriteArray(Frames, static (writer, section) => section.WriteObject(writer));
		
		output.WriteInt32(_colorTop);
		output.WriteInt32(_color);
		output.WriteInt32(_special);
		output.Write(_vertexBoneRefIsSimple);
		output.WriteUInt32(_sizeOfRestData);
		if (_odolVersion >= 50) {
			var floats = _clip.Select(static c => (float) c).ToList();
			output.WriteCondensedIntArray(floats.ToArray());
		}

		UVSets[0].Write(output, _odolVersion);
		output.WriteUInt32(_nUvSets);
		for (var j = 1; j < _nUvSets; j++) UVSets[j].Write(output, _odolVersion);

		output.WriteCompressedArray(Vertices, static (writer, p) => p.WriteObject(writer), 12);
		if (_odolVersion >= 45) {
			var floats = _normals.Select(static c => (Vector3P) c).ToList();
			output.WriteCondensedObjectArray(floats, 4);
		} else output.WriteCondensedObjectArray<Vector3P>(_normals, 12);
		
		if(_odolVersion >= 45)
			output.WriteCompressedArray((STPairCompressed[]) _stCoords, static (writer, compressed) => compressed.WriteObject(writer), 8 );
		else output.WriteCompressedArray((STPairUncompressed[]) _stCoords, static (writer, compressed) => compressed.WriteObject(writer), 24 );

		output.WriteCompressedArray(VertexBoneRef, static (writer, weight) => weight.WriteObject(writer) , 12);
		output.WriteCompressedArray(NeighborBoneRef, static (writer, weight) => weight.WriteObject(writer) , 32);
		if (_odolVersion >= 67) output.WriteUInt32(unknownInt);
		if (_odolVersion >= 68) output.Write((byte) unknownByte);
	} 

	public int CompareTo(LOD other) => resolution.CompareTo(other.resolution);

	private struct PointWeight {
		public int PointIndex;
		public byte Weight;
		
		public PointWeight(int index, byte weight) {
			PointIndex = index;
			this.Weight = weight;
		}
	}
}


