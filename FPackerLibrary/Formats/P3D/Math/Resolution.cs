using FPacker.P3D.Models;

namespace FPacker.P3D.Math; 

public static class Resolution {
	private const float specialLod = 1E+15f;
	public const float GEOMETRY = 1E+13f;
	public const float BUOYANCY = 2E+13f;
	public const float PHYSXOLD = 3E+13f;
	public const float PHYSX = 4E+13f;
	public const float MEMORY = 1E+15f;
	public const float LANDCONTACT = 2E+15f;
	public const float ROADWAY = 3E+15f;
	public const float PATHS = 4E+15f;
	public const float HITPOINTS = 5E+15f;
	public const float VIEW_GEOMETRY = 6E+15f;
	public const float FIRE_GEOMETRY = 7E+15f;
	public const float VIEW_GEOMETRY_CARGO = 8E+15f;
	public const float VIEW_GEOMETRY_PILOT = 1.3E+16f;
	public const float VIEW_GEOMETRY_GUNNER = 1.5E+16f;
	public const float FIRE_GEOMETRY_GUNNER = 1.6E+16f;
	public const float SUBPARTS = 1.7E+16f;
	public const float SHADOWVOLUME_CARGO = 1.8E+16f;
	public const float SHADOWVOLUME_PILOT = 1.9E+16f;
	public const float SHADOWVOLUME_GUNNER = 2E+16f;
	public const float WRECK = 2.1E+16f;
	public const float VIEW_COMMANDER = 1E+16f;
	public const float VIEW_GUNNER = 1000f;
	public const float VIEW_PILOT = 1100f;
	public const float VIEW_CARGO = 1200f;
	public const float SHADOWVOLUME = 10000f;
	public const float SHADOWBUFFER = 11000f;
	public const float SHADOW_MIN = 10000f;
	public const float SHADOW_MAX = 20000f;
	
	public static bool KeepsNamedSelections(float r) => r is 1E+15f or 7E+15f or 1E+13f or 6E+15f or 1.3E+16f or 1.5E+16f or 8E+15f or 4E+15f or 5E+15f or 4E+13f
		or 2E+13f;

	public static LodName getLODType(this float res) {
		switch (res) {
			case 1E+15f:
				return LodName.Memory;
			case 2E+15f:
				return LodName.LandContact;
			case 3E+15f:
				return LodName.Roadway;
			case 4E+15f:
				return LodName.Paths;
			case 5E+15f:
				return LodName.HitPoints;
			case 6E+15f:
				return LodName.ViewGeometry;
			case 7E+15f:
				return LodName.FireGeometry;
			case 8E+15f:
				return LodName.ViewCargoGeometry;
			case 9E+15f:
				return LodName.ViewCargoFireGeometry;
			case 1E+16f:
				return LodName.ViewCommander;
			case 1.1E+16f:
				return LodName.ViewCommanderGeometry;
			case 1.2E+16f:
				return LodName.ViewCommanderFireGeometry;
			case 1.3E+16f:
				return LodName.ViewPilotGeometry;
			case 1.4E+16f:
				return LodName.ViewPilotFireGeometry;
			case 1.4999999E+16f:
				return LodName.ViewGunnerGeometry;
			case 1.6E+16f:
				return LodName.ViewGunnerFireGeometry;
			case 1.7E+16f:
				return LodName.SubParts;
			case 1.8E+16f:
				return LodName.ShadowVolumeViewCargo;
			case 1.9E+16f:
				return LodName.ShadowVolumeViewPilot;
			case 2E+16f:
				return LodName.ShadowVolumeViewGunner;
			case 2.1E+16f:
				return LodName.Wreck;
			case 1000f:
				return LodName.ViewGunner;
			case 1100f:
				return LodName.ViewPilot;
			case 1200f:
				return LodName.ViewCargo;
			case 1E+13f:
				return LodName.Geometry;
			case 4E+13f:
				return LodName.PhysX;
		}

		if (res >= 10000.0 && res <= 20000.0) return LodName.ShadowVolume;
		return LodName.Resolution;
	}

	public static string getLODName(this float res) {
		var lodType = res.getLODType();
		return ((lodType != LodName.ShadowVolume) 
			? lodType == LodName.Resolution 
				? res.ToString("#.000") 
				: Enum.GetName(typeof(LodName), lodType) 
			: "ShadowVolume" + (res - 10000f).ToString("#.000")) ?? string.Empty;;
	}

	public static bool IsResolution(float r) => r < 10000f;

	public static bool IsShadow(float r) => r is >= 10000f and < 20000f or 2E+16f or 1.9E+16f or 1.8E+16f;

	public static bool IsVisual(float r) => IsResolution(r) || r is 1200f or 1000f or 1100f or 1E+16f;
}