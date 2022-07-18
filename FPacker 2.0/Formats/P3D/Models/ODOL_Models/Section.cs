using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL;

public class Section : IDeserializable {
	private int _faceLowerIndex;
	private int _faceUpperIndex;
	private int _minBoneIndex;
	private int _bonesCount;
	public short TextureIndex;
	public uint Special;
	public int MaterialIndex;
	private string _mat;
	private uint _nStages;
	private float[] _areaOverTex;
	private bool _shortIndices;
	public float[] unknownArr;

	public uint unknown;
	public int unknown2;


	public uint[] getFaceIndexes(Polygon[] faces) {
		var num = 0U;
		var num2 = this._shortIndices ? 8U : 16U;
		var num3 = this._shortIndices ? 2U : 4U;
		var list = new List<uint>();
		var num4 = 0U;
		while ((ulong) num4 < (ulong) ((long) faces.Length)) {
			if ((ulong) num >= (ulong) ((long) this._faceLowerIndex) &&
			    (ulong) num < (ulong) ((long) this._faceUpperIndex)) list.Add(num4);

			num += num2;
			if (faces[(int) num4].VertexIndices.Length == 4) num += num3;

			if ((ulong) num >= (ulong) ((long) this._faceUpperIndex)) break;

			num4 += 1U;
		}

		return list.ToArray();
	}

	public void ReadObject(P3DBinaryReader input) {
		int version = input.Version;
		this._shortIndices = (version < 69);
		this._faceLowerIndex = input.ReadInt32();
		this._faceUpperIndex = input.ReadInt32();
		this._minBoneIndex = input.ReadInt32();
		this._bonesCount = input.ReadInt32();
		unknown = input.ReadUInt32();
		this.TextureIndex = input.ReadInt16();
		this.Special = input.ReadUInt32();
		this.MaterialIndex = input.ReadInt32();
		if (this.MaterialIndex == -1) this._mat = input.ReadAsciiZ();

		if (version >= 36) {
			this._nStages = input.ReadUInt32();
			this._areaOverTex = new float[this._nStages];
			var num = 0;
			while ((long) num < (long) ((ulong) this._nStages)) {
				this._areaOverTex[num] = input.ReadSingle();
				num++;
			}

			if (version < 67 || (unknown2 = input.ReadInt32()) < 1) return;
			unknownArr = Enumerable.Range(0, 11).Select(_ => input.ReadSingle()).ToArray<float>();
			return;
		} else {
			this._areaOverTex = new float[1];
			this._areaOverTex[0] = input.ReadSingle();
		}
	}

	public void WriteObject(P3DBinaryWriter output) {
		output.WriteInt32(_faceLowerIndex);
		output.WriteInt32(_faceUpperIndex);
		output.WriteInt32(_minBoneIndex);
		output.WriteInt32(_bonesCount);
		output.WriteUInt32(unknown);
		output.Write(TextureIndex);
		output.WriteUInt32(Special);
		output.WriteInt32(MaterialIndex);
		if (this.MaterialIndex == -1) output.WriteAsciiZ(_mat);
		if (output.Version >= 36U) {
			output.WriteUInt32(_nStages);
			var num = 0;
			while ((long) num < (long) ((ulong) this._nStages)) {
				output.WriteSingle(_areaOverTex[num]);
				num++;
			}
			output.WriteInt32(unknown2);
			if (output.Version < 67 || unknown2 < 1) return;
			foreach (var f in unknownArr) output.WriteSingle(f);
			return;
		} else output.WriteSingle(_areaOverTex[0]);
	}
}