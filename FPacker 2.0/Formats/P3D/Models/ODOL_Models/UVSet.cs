using FPacker.P3D.IO;

namespace FPacker.P3D.Models.ODOL;

public class UVSet {
	
	// Token: 0x0400018F RID: 399
	private bool _isDiscretized;
	private float _minU;
	private float _minV;
	private float _maxU;
	private float _maxV;
	private uint _nVertices;
	private bool _defaultFill;
	private byte[] _defaultValue;

	private byte[] _uvData;
	
	public float[] UVData {
		get {
			var array = new float[this._nVertices * 2U];
			var num = 0f;
			var num2 = 0f;
			var scale = 1.0;
			var scale2 = 1.0;
			if (this._isDiscretized) {
				scale = (double) (this._maxU - this._minU);
				scale2 = (double) (this._maxV - this._minV);
			}

			if (this._defaultFill) {
				if (this._isDiscretized) {
					num = this.Scale(BitConverter.ToInt16(this._defaultValue, 0), scale, this._minU);
					num2 = this.Scale(BitConverter.ToInt16(this._defaultValue, 2), scale2, this._minV);
				}
				else {
					num = BitConverter.ToSingle(this._defaultValue, 0);
					num2 = BitConverter.ToSingle(this._defaultValue, 4);
				}
			}

			int num3 = 0;
			while ((long) num3 < (long) ((ulong) this._nVertices)) {
				if (this._isDiscretized) {
					array[num3 * 2] = (this._defaultFill
						? num
						: this.Scale(BitConverter.ToInt16(this._uvData, num3 * 4), scale, this._minU));
					array[num3 * 2 + 1] = (this._defaultFill
						? num2
						: this.Scale(BitConverter.ToInt16(this._uvData, num3 * 4 + 2), scale2, this._minV));
				}
				else {
					array[num3 * 2] = (this._defaultFill ? num : BitConverter.ToSingle(this._uvData, num3 * 8));
					array[num3 * 2 + 1] = (this._defaultFill ? num2 : BitConverter.ToSingle(this._uvData, num3 * 8 + 4));
				}

				num3++;
			}

			return array;
		}
	}

	private float Scale(short value, double scale, float min) {
		return (float) (1.52587890625E-05 * (double) (value + short.MaxValue) * scale) + min;
	}

	public void Read(P3DBinaryReader input, uint odolVersion) {
		this._isDiscretized = false;
		if (odolVersion >= 45U) {
			this._isDiscretized = true;
			this._minU = input.ReadSingle();
			this._minV = input.ReadSingle();
			this._maxU = input.ReadSingle();
			this._maxV = input.ReadSingle();
		}

		this._nVertices = input.ReadUInt32();
		this._defaultFill = input.ReadBoolean();
		var num = (odolVersion >= 45U) ? 4 : 8;
		if (this._defaultFill) {
			this._defaultValue = input.ReadBytes(num);
			return;
		}

		this._uvData = input.ReadCompressed((uint) ((ulong) this._nVertices * (ulong) ((long) num)));
	}

	public void Write(P3DBinaryWriter output, uint odolVersion) {
		if (odolVersion >= 45U) {
			output.WriteSingle(_minU);
			output.WriteSingle(_minV);
			output.WriteSingle(_maxU);
			output.WriteSingle(_maxV);
		}
		
		output.WriteUInt32(_nVertices);
		output.Write(_defaultFill);
		
		if (this._defaultFill) {
			output.Write(_defaultValue);
			return;
		}
		output.WriteCompressed(_uvData);
	}
}