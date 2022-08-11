using FPackerLibrary.P3D.IO;
using FPackerLibrary.P3D.Math;

namespace FPackerLibrary.P3D.Models.ODOL;

public class ODOL_ModelInfo {
	public int special { get; private set; }
	public float BoundingSphere { get; private set; }
	public float GeometrySphere { get; private set; }
	public int Remarks { get; private set; }
	public int AndHints { get; private set; }
	public int OrHints { get; private set; }
	public Vector3P AimingCenter { get; private set; }
	public PackedColor Color { get; private set; }
	public PackedColor ColorType { get; private set; }
	public float ViewDensity { get; private set; }
	public Vector3P BboxMin { get; private set; }
	public Vector3P BboxMax { get; private set; }
	public float PropertyLodDensityCoefficient { get; private set; }
	public float PropertyDrawImportance { get; private set; }
	public Vector3P BboxMinVisual { get; private set; }
	public Vector3P BboxMaxVisual { get; private set; }
	public Vector3P BoundingCenter { get; private set; }
	public Vector3P GeometryCenter { get; private set; }
	public Vector3P CenterOfMass { get; private set; }
	public Matrix3P InvInertia { get; private set; }
	public bool AutoCenter { get; private set; }
	public bool LockAutoCenter { get; private set; }
	public bool CanOcclude { get; private set; }
	public bool CanBeOccluded { get; private set; }
	public bool AiCovers { get; private set; }
	public float HtMin { get; private set; }
	public float HtMax { get; private set; }
	public float AfMax { get; private set; }
	public float MfMax { get; private set; }
	public float MFact { get; private set; }
	public float TBody { get; private set; }
	public bool ForceNotAlphaModel { get; private set; }
	public SBSource SbSource { get; private set; }
	public bool PreferShadowVolume { get; private set; }
	public float ShadowOffset { get; private set; }
	public bool Animated { get; private set; }
	public Skeleton Skeleton { get; private set; }
	public MapType MapType { get; private set; }
	public float[] MassArray { get; private set; }
	public float Mass { get; private set; }
	public float InvMass { get; private set; }
	public float Armor { get; private set; }
	public float InvArmor { get; private set; }
	public float PropertyExplosionShielding { get; private set; }
	public byte Memory { get; private set; }
	public byte Geometry { get; private set; }
	public byte GeometrySimple { get; private set; }
	public byte GeometryPhys { get; private set; }
	public byte GeometryFire { get; private set; }
	public byte GeometryView { get; private set; }
	public byte GeometryViewPilot { get; private set; }
	public byte GeometryViewGunner { get; private set; }
	public byte GeometryViewCargo { get; private set; }
	public byte LandContact { get; private set; }
	public byte Roadway { get; private set; }
	public byte Paths { get; private set; }
	public byte HitPoints { get; private set; }
	public byte MinShadow { get; private set; }
	public bool CanBlend { get; private set; }
	public string PropertyClass { get; private set; }
	public string PropertyDamage { get; private set; }
	public bool PropertyFrequent { get; private set; }
	public int[] PreferredShadowVolumeLod { get; private set; }
	public int[] PreferredShadowBufferLod { get; private set; }
	public int[] PreferredShadowBufferLodVis { get; private set; }
	
	public byte[] UnknownBytes { get; set; }
	public sbyte UnknownSByte { get; set; }
	public uint UnknownUInt { get; set; }

	internal ODOL_ModelInfo(P3DBinaryReader input, uint nLods) => 
		Read(input, nLods);
	
	public void Read(P3DBinaryReader input, uint nLods) {
		var version = input.Version;
		special = input.ReadInt32();
		this.BoundingSphere = input.ReadSingle();
		this.GeometrySphere = input.ReadSingle();
		this.Remarks = input.ReadInt32();
		this.AndHints = input.ReadInt32();
		this.OrHints = input.ReadInt32();
		this.AimingCenter = new Vector3P(input);
		this.Color = new PackedColor(input.ReadUInt32());
		this.ColorType = new PackedColor(input.ReadUInt32());
		this.ViewDensity = input.ReadSingle();
		this.BboxMin = new Vector3P(input);
		this.BboxMax = new Vector3P(input);
		if (version >= 70) PropertyLodDensityCoefficient = input.ReadSingle();

		if (version >= 71) PropertyDrawImportance = input.ReadSingle();

		if (version >= 52) {
			this.BboxMinVisual = new Vector3P(input);
			this.BboxMaxVisual = new Vector3P(input);
		}
		this.BoundingCenter = new Vector3P(input);
		this.GeometryCenter = new Vector3P(input);
		this.CenterOfMass = new Vector3P(input);
		this.InvInertia = new Matrix3P(input);
		this.AutoCenter = input.ReadBoolean();
		this.LockAutoCenter = input.ReadBoolean();
		this.CanOcclude = input.ReadBoolean();
		this.CanBeOccluded = input.ReadBoolean();
		if (version >= 73) AiCovers = input.ReadBoolean();

		if (version >= 53) UnknownBytes = input.ReadBytes(5);

		if (version is >= 42 and < 10000 or >= 10042) {
			this.HtMin = input.ReadSingle();
			this.HtMax = input.ReadSingle();
			this.AfMax = input.ReadSingle();
			this.MfMax = input.ReadSingle();
		}

		if (version is >= 43 and < 10000 or >= 10043) {
			this.MFact = input.ReadSingle();
			this.TBody = input.ReadSingle();
		}

		if (version >= 33) ForceNotAlphaModel = input.ReadBoolean();

		if (version >= 37) {
			var num = input.ReadInt32();
			this.SbSource = (SBSource) num;
			this.PreferShadowVolume = input.ReadBoolean();
		}

		if (version >= 48) ShadowOffset = input.ReadSingle();
		this.Animated = input.ReadBoolean();
		this.Skeleton = new Skeleton(input);
		this.MapType = (MapType) input.ReadByte();
		this.MassArray = input.ReadCompressedFloatArray();
		this.Mass = input.ReadSingle();
		this.InvMass = input.ReadSingle();
		this.Armor = input.ReadSingle();
		this.InvArmor = input.ReadSingle();
		if (version >= 72) PropertyExplosionShielding = input.ReadSingle();
		if (version > 53) GeometrySimple = input.ReadByte();
		if (version >= 54) GeometryPhys = input.ReadByte();
		this.Memory = input.ReadByte();
		this.Geometry = input.ReadByte();
		this.GeometryFire = input.ReadByte();
		this.GeometryView = input.ReadByte();
		this.GeometryViewPilot = input.ReadByte();
		this.GeometryViewGunner = input.ReadByte();
		UnknownSByte = input.ReadSByte();
		this.GeometryViewCargo = input.ReadByte();
		this.LandContact = input.ReadByte();
		this.Roadway = input.ReadByte();
		this.Paths = input.ReadByte();
		this.HitPoints = input.ReadByte();
		this.MinShadow = (byte) input.ReadUInt32();
		if (version >= 38) CanBlend = input.ReadBoolean();
		this.PropertyClass = input.ReadAsciiZ();
		this.PropertyDamage = input.ReadAsciiZ();
		this.PropertyFrequent = input.ReadBoolean();
		if (version >= 31) UnknownUInt = input.ReadUInt32();

		if (version < 57) return;
		this.PreferredShadowVolumeLod = new int[nLods];
		this.PreferredShadowBufferLod = new int[nLods];
		this.PreferredShadowBufferLodVis = new int[nLods];
		for (var i = 0; i < nLods; i++) PreferredShadowVolumeLod[i] = input.ReadInt32();
		for (var j = 0; j < nLods; j++) PreferredShadowBufferLod[j] = input.ReadInt32();
		for (var k = 0; k < nLods; k++) PreferredShadowBufferLodVis[k] = input.ReadInt32();
	}

	public void Write(P3DBinaryWriter output, uint nLods) {
		output.WriteInt32(special);
		output.WriteSingle(BoundingSphere);
		output.WriteSingle(GeometrySphere);
		output.WriteInt32(Remarks);
		output.WriteInt32(AndHints);
		output.WriteInt32(OrHints);
		AimingCenter.WriteObject(output);
		output.WriteUInt32(Color.Value);
		output.WriteUInt32(ColorType.Value);
		output.WriteSingle(ViewDensity);
		BboxMin.WriteObject(output);
		BboxMax.WriteObject(output);
		
		if (output.Version >= 70U) output.WriteSingle(PropertyLodDensityCoefficient);
		if (output.Version >= 71U) output.WriteSingle(PropertyDrawImportance);

		if (output.Version >= 52U) {
			BboxMinVisual.WriteObject(output);
			BboxMaxVisual.WriteObject(output);
		}
		
		BoundingCenter.WriteObject(output);
		GeometryCenter.WriteObject(output);
		CenterOfMass.WriteObject(output);
		InvInertia.WriteObject(output);
		output.Write(AutoCenter);
		output.Write(LockAutoCenter);
		output.Write(CanOcclude);
		output.Write(CanBeOccluded);
		if (output.Version >= 73U) output.Write(AiCovers);
		if (output.Version >= 53U) output.Write(UnknownBytes);
		
		if (output.Version is >= 42U and < 10000U or >= 10042U) {
			output.WriteSingle(HtMin);
			output.WriteSingle(HtMax);
			output.WriteSingle(AfMax);
			output.WriteSingle(MfMax);
		}
		
		if (output.Version is >= 43U and < 10000U or >= 10043U) {
			output.WriteSingle(MFact);
			output.WriteSingle(TBody);
		}
		
		if (output.Version >= 33U) output.Write(ForceNotAlphaModel);
		
		if (output.Version >= 37U) {
			output.WriteInt32((int) SbSource);
			output.Write(PreferShadowVolume);
		}
		
		if (output.Version >= 48U) output.WriteSingle(ShadowOffset);
		output.Write(Animated);
		Skeleton.Write(output);
		output.Write((byte) MapType);
		output.WriteCompressedFloatArray(MassArray);
		output.WriteSingle(Mass);
		output.WriteSingle(InvMass);
		output.WriteSingle(Armor);
		output.WriteSingle(InvArmor);
		if (output.Version >= 72U) output.WriteSingle(PropertyExplosionShielding);
		if (output.Version > 53U) output.WriteSingle(GeometrySimple);
		if (output.Version >= 54U) output.WriteSingle(GeometryPhys);
		output.Write(Memory);
		output.Write(Geometry);
		output.Write(GeometryFire);
		output.Write(GeometryView);
		output.Write(GeometryViewPilot);
		output.Write(GeometryViewGunner);
		output.Write(UnknownSByte);
		output.Write(GeometryViewCargo);
		output.Write(LandContact);
		output.Write(Roadway);
		output.Write(Paths);
		output.Write(HitPoints);
		output.WriteUInt32(MinShadow);
		if (output.Version >= 38U) output.Write(CanBlend);
		output.WriteAsciiZ(PropertyClass);
		output.WriteAsciiZ(PropertyDamage);
		output.Write(PropertyFrequent);
		if (output.Version >= 31) output.WriteUInt32(UnknownUInt);
		if (output.Version < 57U) return;
		for (var i = 0; i < nLods; i++) output.Write(PreferredShadowVolumeLod[i]);
		for (var i = 0; i < nLods; i++) output.Write(PreferredShadowBufferLod[i]);
		for (var i = 0; i < nLods; i++) output.Write(PreferredShadowBufferLodVis[i]);
		
	}
}