using FPacker.P3D.IO;
using FPacker.P3D.Math;

namespace FPacker.P3D.Models.ODOL;

public class Animations {
	public AnimationClass[] AnimationClasses;
	public int NAnimLoDs;
	public uint[][][] Bones2Anims;
	public int[][] Anims2Bones;
	public Vector3P[][][] AxisData;

	public void Read(P3DBinaryReader input) {
		AnimationClasses = input.ReadArray<AnimationClass>();
		var num = AnimationClasses.Length;
		NAnimLoDs = input.ReadInt32();
		Bones2Anims = new uint[NAnimLoDs][][];
		for (var i = 0; i < NAnimLoDs; i++) {
			var num2 = input.ReadUInt32();
			Bones2Anims[i] = new uint[num2][];
			var num3 = 0;
			while (num3 < (long) num2) {
				var num4 = input.ReadUInt32();
				Bones2Anims[i][num3] = new uint[num4];
				var num5 = 0;
				while (num5 < (long) num4) {
					Bones2Anims[i][num3][num5] = input.ReadUInt32();
					num5++;
				}

				num3++;
			}
		}

		Anims2Bones = new int[NAnimLoDs][];
		AxisData = new Vector3P[NAnimLoDs][][];
		for (var j = 0; j < NAnimLoDs; j++) {
			Anims2Bones[j] = new int[num];
			AxisData[j] = new Vector3P[num][];
			for (var k = 0; k < num; k++) {
				Anims2Bones[j][k] = input.ReadInt32();
				if (Anims2Bones[j][k] == -1 || AnimationClasses[k].AnimationType is AnimationClass.AnimType.Direct or AnimationClass.AnimType.Hide) continue;
				AxisData[j][k] = new Vector3P[2];
				AxisData[j][k][0] = new Vector3P(input);
				AxisData[j][k][1] = new Vector3P(input);
			}
		}
	}
	
	
	public void Write(P3DBinaryWriter output) {
		output.WriteArray(AnimationClasses, static (writer, animClass) => animClass.WriteObject(writer));
		output.WriteInt32(NAnimLoDs);
		for (var i = 0; i < NAnimLoDs; i++) {
			var bones2AnimsLengthX = (uint) Bones2Anims[i].Length;
			output.WriteUInt32(bones2AnimsLengthX);
			var j = 0;
			while (j < bones2AnimsLengthX) {
				var bones2AnimsLengthY = (uint) Bones2Anims[i][j].Length;
				output.WriteUInt32(bones2AnimsLengthY);
				var k = 0;
				while (k < bones2AnimsLengthY) {
					output.WriteUInt32(Bones2Anims[i][j][k]);
					k++;
				}
				j++;
			}
		}
		
		for (var j = 0; j < NAnimLoDs; j++) {
			for (var k = 0; k < AnimationClasses.Length; k++) {
				output.WriteInt32(Anims2Bones[j][k]);
				if (Anims2Bones[j][k] == -1 || AnimationClasses[k].AnimationType is AnimationClass.AnimType.Direct or AnimationClass.AnimType.Hide) continue;
				AxisData[j][k][0].WriteObject(output);
				AxisData[j][k][1].WriteObject(output);
			}
		}
	}

	public class AnimationClass : IDeserializable {
		public AnimType AnimationType;
		public string AnimName;
		public string AnimSource;
		public float MinValue;
		public float MaxValue;
		public float MinPhase;
		public float MaxPhase;
		public float AnimPeriod;
		public float InitPhase;
		public AnimAddress SourceAddress;
		public float Angle0;
		public float Angle1;
		public float Offset0;
		public float Offset1;
		public Vector3P AxisPos;
		public Vector3P AxisDir;
		public float Angle;
		public float AxisOffset;
		public float HideValue;

		public float UnknownSingle;

		public void ReadObject(P3DBinaryReader input) {
			var version = input.Version;
			AnimationType = (AnimType) input.ReadUInt32();
			AnimName = input.ReadAsciiZ();
			AnimSource = input.ReadAsciiZ();
			MinPhase = input.ReadSingle();
			MaxPhase = input.ReadSingle();
			MinValue = input.ReadSingle();
			MaxValue = input.ReadSingle();
			if (version >= 56) {
				AnimPeriod = input.ReadSingle();
				InitPhase = input.ReadSingle();
			}

			SourceAddress = (AnimAddress) input.ReadUInt32();
			switch (AnimationType) {
				case AnimType.Rotation:
				case AnimType.RotationX:
				case AnimType.RotationY:
				case AnimType.RotationZ:
					Angle0 = input.ReadSingle();
					Angle1 = input.ReadSingle();
					return;
				case AnimType.Translation:
				case AnimType.TranslationX:
				case AnimType.TranslationY:
				case AnimType.TranslationZ:
					Offset0 = input.ReadSingle();
					Offset1 = input.ReadSingle();
					return;
				case AnimType.Direct:
					AxisPos = new Vector3P(input);
					AxisDir = new Vector3P(input);
					Angle = input.ReadSingle();
					AxisOffset = input.ReadSingle();
					return;
				case AnimType.Hide:
					HideValue = input.ReadSingle();
					if (version < 55) return;
					UnknownSingle = input.ReadSingle();
					return;
				default:
					throw new Exception("Unknown AnimType encountered: " + AnimationType);
			}
		}
		
		public void WriteObject(P3DBinaryWriter output) {
			output.WriteUInt32((uint) AnimationType);
			output.WriteAsciiZ(AnimName);
			output.WriteAsciiZ(AnimSource);
			output.WriteSingle(MinPhase);
			output.WriteSingle(MaxPhase);
			output.WriteSingle(MinValue);
			output.WriteSingle(MaxValue);
			if (output.Version >= 56U) {
				output.WriteSingle(AnimPeriod);
				output.WriteSingle(InitPhase);
			}
			output.WriteUInt32((uint) SourceAddress);

			switch (AnimationType) {
				case AnimType.Rotation:
				case AnimType.RotationX:
				case AnimType.RotationY:
				case AnimType.RotationZ:
					output.WriteSingle(Angle0);
					output.WriteSingle(Angle1);
					return;
				case AnimType.Translation:
				case AnimType.TranslationX:
				case AnimType.TranslationY:
				case AnimType.TranslationZ:
					output.WriteSingle(Offset0);
					output.WriteSingle(Offset1);
					return;
				case AnimType.Direct:
					AxisPos.WriteObject(output);
					AxisDir.WriteObject(output);
					output.WriteSingle(Angle);
					output.WriteSingle(AxisOffset);
					return;
				case AnimType.Hide:
					output.WriteSingle(HideValue);
					if (output.Version < 55U) return;
					output.WriteSingle(UnknownSingle);
					return;
				default:
					throw new Exception("Unknown AnimType encountered: " + AnimationType);
			}
		}
		
		public enum AnimType {
			Rotation,
			RotationX,
			RotationY,
			RotationZ,
			Translation,
			TranslationX,
			TranslationY,
			TranslationZ,
			Direct,
			Hide
		}

		public enum AnimAddress {
			AnimClamp,
			AnimLoop,
			AnimMirror,
			NAnimAddress
		}

		
	}

}