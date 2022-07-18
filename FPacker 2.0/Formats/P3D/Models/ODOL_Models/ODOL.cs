using FPacker.P3D.IO;
using FPacker.P3D.Math;
namespace FPacker.P3D.Models.ODOL;

public class ODOL : P3D {
	public const int LatestVersion = 73;
	public const int MinimalVersion = 28;
	private string _muzzleFlash;
	private uint _appId;
	public int _lodCount;
	private float[] _resolutions;
	public ODOL_ModelInfo ModelInfo;
	private bool _hasAnims;
	private readonly Animations animations = new();
	private uint[] _lodStartAddresses;
	private uint[] _lodEndAddresses;
	private bool[] _permanent;
	private List<LoadableLodInfo> _loadableLodInfos;
	private LOD[] _lods;
	private int buoyancyType;
	
	public Skeleton Skeleton => this.ModelInfo.Skeleton;

	public override float Mass => this.ModelInfo.Mass;

	public override P3D_LOD[] LODs => this._lods;

	public readonly string FileName;

	public ODOL(string fileName) {
		FileName = fileName; 
		Stream stream = File.OpenRead(fileName);
		Read(new P3DBinaryReader(stream));
	}

	public bool IsSnappable() {
		var lod = this._lods.FirstOrDefault(static  l => l.Resolution.getLODType() == LodName.Memory);
		if (lod == null) return false;
		return (from ns in lod.NamedSelections
			where ns.Name.Equals("lb", StringComparison.InvariantCultureIgnoreCase) ||
			      ns.Name.Equals("le", StringComparison.InvariantCultureIgnoreCase) ||
			      ns.Name.Equals("pb", StringComparison.InvariantCultureIgnoreCase) ||
			      ns.Name.Equals("pe", StringComparison.InvariantCultureIgnoreCase)
			select ns).Count<NamedSelection>() >= 4;
	}
	
	public void Write(P3DBinaryWriter output) {
		output.Version = Version;

		output.WriteAscii("ODOL", 4);
		output.WriteUInt32(Version);
		
		switch (Version) {
			case > 73U:
				throw new FormatException("Unknown ODOL version");
			case < 28U:
				throw new FormatException("Old ODOL version is currently not supported");
		}
		if (Version >= 44U) output.UseLZOCompression = true;
		if (Version >= 64U) output.UseCompressionFlag = true;
		if (Version >= 59U) output.WriteUInt32(_appId);
		if (Version >= 58U) output.WriteAsciiZ(_muzzleFlash);
		
		output.WriteUInt32((uint) LODs.Length);
		foreach (var resolution in _resolutions) output.WriteSingle(resolution);
		
		//ModelInfo
		WriteModelInfo(output);
		if (Version >= 30U) {
			output.Write(_hasAnims);
			if (_hasAnims) animations.Write(output);
		}

		for (var i = 0; i < LODs.Length; i++) output.WriteUInt32(_lodStartAddresses[i]);
		for (var i = 0; i < LODs.Length; i++) output.WriteUInt32(_lodEndAddresses[i]);
		for (var i = 0; i < LODs.Length; i++) output.Write(_permanent[i]);
		
		var position = output.Position;
		for (var i = 0; i < _loadableLodInfos.Count; i++) {
			if (!_permanent[i]) {
				_loadableLodInfos[i].WriteObject(output);
				position = output.Position;
			}

			output.Position = _lodStartAddresses[i];
			_lods[i].Write(output, _resolutions[i]);
		}
		if (Version >= 54U) {
			output.Position = _lodEndAddresses.Max();
			output.WriteInt32(buoyancyType);
		}
		
		output.Close();
	}

	private void WriteModelInfo(P3DBinaryWriter output) {
		output.WriteInt32(ModelInfo.special);
		output.WriteSingle(ModelInfo.BoundingSphere);
		output.WriteSingle(ModelInfo.GeometrySphere);
		output.WriteInt32(ModelInfo.Remarks);
		output.WriteInt32(ModelInfo.AndHints);
		ModelInfo.AimingCenter.WriteObject(output);
		output.WriteUInt32(ModelInfo.Color.Value);
		output.WriteUInt32(ModelInfo.ColorType.Value);
		output.WriteSingle(ModelInfo.ViewDensity);
		ModelInfo.BboxMin.WriteObject(output);
		ModelInfo.BboxMax.WriteObject(output);
		
		if (Version >= 70U) output.WriteSingle(ModelInfo.PropertyLodDensityCoefficient);
		if (Version >= 71U) output.WriteSingle(ModelInfo.PropertyDrawImportance);

		if (Version >= 52U) {
			ModelInfo.BboxMinVisual.WriteObject(output);
			ModelInfo.BboxMaxVisual.WriteObject(output);
		}
		
		ModelInfo.BoundingCenter.WriteObject(output);
		ModelInfo.GeometryCenter.WriteObject(output);
		ModelInfo.CenterOfMass.WriteObject(output);
		ModelInfo.InvInertia.WriteObject(output);
		output.Write(ModelInfo.AutoCenter);
		output.Write(ModelInfo.LockAutoCenter);
		output.Write(ModelInfo.CanOcclude);
		output.Write(ModelInfo.CanBeOccluded);
		if (Version >= 73U) output.Write(ModelInfo.AiCovers);
		if (Version >= 53U) output.Write(ModelInfo.UnknownBytes);
		
		if (Version is >= 42U and < 10000U or >= 10042U) {
			output.WriteSingle(ModelInfo.HtMin);
			output.WriteSingle(ModelInfo.HtMax);
			output.WriteSingle(ModelInfo.AfMax);
			output.WriteSingle(ModelInfo.MfMax);
		}
		
		if (Version is >= 43U and < 10000U or >= 10043U) {
			output.WriteSingle(ModelInfo.MFact);
			output.WriteSingle(ModelInfo.TBody);
		}
		
		if (Version >= 33U) output.Write(ModelInfo.ForceNotAlphaModel);
		
		if (Version >= 37U) {
			output.WriteInt32((int) ModelInfo.SbSource);
			output.Write(ModelInfo.PreferShadowVolume);
		}
		
		if (Version >= 48U) output.WriteSingle(ModelInfo.ShadowOffset);
		output.Write(ModelInfo.Animated);
		ModelInfo.Skeleton.Write(output);
		output.Write((byte) ModelInfo.MapType);
		output.WriteCompressedFloatArray(ModelInfo.MassArray);
		output.WriteSingle(ModelInfo.Mass);
		output.WriteSingle(ModelInfo.InvMass);
		output.WriteSingle(ModelInfo.Armor);
		output.WriteSingle(ModelInfo.InvArmor);
		if (Version >= 72U) output.WriteSingle(ModelInfo.PropertyExplosionShielding);
		if (Version > 53U) output.WriteSingle(ModelInfo.GeometrySimple);
		if (Version >= 54U) output.WriteSingle(ModelInfo.GeometryPhys);
		output.Write(ModelInfo.Memory);
		output.Write(ModelInfo.Geometry);
		output.Write(ModelInfo.GeometryFire);
		output.Write(ModelInfo.GeometryView);
		output.Write(ModelInfo.GeometryViewPilot);
		output.Write(ModelInfo.GeometryViewGunner);
		output.Write(ModelInfo.UnknownSByte);
		output.Write(ModelInfo.GeometryViewCargo);
		output.Write(ModelInfo.LandContact);
		output.Write(ModelInfo.Roadway);
		output.Write(ModelInfo.Paths);
		output.Write(ModelInfo.HitPoints);
		output.WriteUInt32(ModelInfo.MinShadow);
		if (Version >= 38U) output.Write(ModelInfo.CanBlend);
		output.WriteAsciiZ(ModelInfo.PropertyClass);
		output.WriteAsciiZ(ModelInfo.PropertyDamage);
		output.Write(ModelInfo.PropertyFrequent);
		output.WriteUInt32(ModelInfo.UnknownUInt);
		if (Version < 57U) return;
		ModelInfo.PreferredShadowVolumeLod.ToList().ForEach(output.WriteInt32);
		ModelInfo.PreferredShadowBufferLod.ToList().ForEach(output.WriteInt32);
		ModelInfo.PreferredShadowBufferLodVis.ToList().ForEach(output.WriteInt32);
	}


	private void Read(P3DBinaryReader input) {
		var b = input.ReadAscii(4);
		if ("ODOL" != b) throw new FormatException("ODOL signature is missing");
		Version = input.ReadUInt32();
		//Console.WriteLine("Version is " + base.Version.ToString() + ", Hex: " + Version.ToString("X4"));
		switch (Version) {
			case > 73U:
				throw new FormatException("Unknown ODOL version");
			case < 28U:
				throw new FormatException("Old ODOL version is currently not supported");
		}

		input.Version = (int) Version;
		if (Version >= 44U) input.UseLZOCompression = true;
		if (Version >= 64U) input.UseCompressionFlag = true;
		if (Version >= 59U) _appId = input.ReadUInt32();
		if (Version >= 58U) _muzzleFlash = input.ReadAsciiZ();
		_lodCount = input.ReadInt32();
		_resolutions = new float[_lodCount];
		for (var i = 0; i < _lodCount; i++) _resolutions[i] = input.ReadSingle();
		ModelInfo = new ODOL_ModelInfo(input, _lodCount);
		if (Version >= 30U) {
			_hasAnims = input.ReadBoolean();
			if (_hasAnims) animations.Read(input);
		}
		_lodStartAddresses = new uint[_lodCount];
		_lodEndAddresses = new uint[_lodCount];
		_permanent = new bool[_lodCount];
		for (var j = 0; j < _lodCount; j++) _lodStartAddresses[j] = input.ReadUInt32();
		for (var k = 0; k < _lodCount; k++) _lodEndAddresses[k] = input.ReadUInt32();

		for (var l = 0; l < _lodCount; l++) {
			_permanent[l] = input.ReadBoolean();
		}

		_loadableLodInfos = new List<LoadableLodInfo>(_lodCount);
		_lods = new LOD[_lodCount];
		var position = input.Position;
		for (var m = 0; m < _lodCount; m++) {
			if (!_permanent[m]) {
				var loadableLodInfo = new LoadableLodInfo();
				loadableLodInfo.ReadObject(input);
				_loadableLodInfos.Add(loadableLodInfo);
				position = input.Position;
			}

			input.Position = _lodStartAddresses[m];
			_lods[m] = new LOD();
			_lods[m].Read(input, _resolutions[m]);
			input.Position = position;
		}

		if (Version >= 54U) {
			input.Position = _lodEndAddresses.Max();
			buoyancyType = input.ReadInt32();
		}

		input.Close();
	}

	public string[] GetHiddenSelectionNames() {
		var ret = new List<string>();
		for (var idxLod = 0; idxLod < _lodCount; idxLod++) {
			if (!_lods[idxLod].Name.StartsWith(".")) continue;
			ret.AddRange(_lods[idxLod].Sections.Select(static section => ""));

			for (var idxNamedSelection = 0;
			     idxNamedSelection < _lods[idxLod].NamedSelections.Count();
			     idxNamedSelection++) {
				if (_lods[idxLod].NamedSelections[idxNamedSelection].Sections.Length == 1) {
					ret[_lods[idxLod].NamedSelections[idxNamedSelection].Sections[0]] =
						_lods[idxLod].NamedSelections[idxNamedSelection].Name;
				}
			}
		}

		return ret.Where(static selection => selection != "").ToArray();
	}

	public string[] GetAxisSelectionNames() {
		var ret = new List<string>();

		for (var idxLod = 0; idxLod < _lodCount; idxLod++) {
			if (_lods[idxLod].Name != "Memory") continue;
			for (var idxAnimClass = 0; idxAnimClass < animations.AxisData[idxLod].Length; idxAnimClass++) {
				if (animations.AxisData[idxLod][idxAnimClass] == null) {
					ret.Add("");
					continue;
				}

				foreach (var selection in _lods[idxLod].NamedSelections) {
					if (selection.SelectedVertices.Count() == 2) {
						switch (animations.AnimationClasses[idxAnimClass].AnimationType) {
							case Animations.AnimationClass.AnimType.Rotation:
							case Animations.AnimationClass.AnimType.RotationX:
							case Animations.AnimationClass.AnimType.RotationY:
							case Animations.AnimationClass.AnimType.RotationZ: {
								Vector3P r = _lods[idxLod].Points[selection.SelectedVertices[0].Value];
								if (AreVectorsAboutEqual(r, animations.AxisData[idxLod][idxAnimClass][0])) {
									ret.Add(selection.Name);
									break;
								}

								continue;
							}
							case Animations.AnimationClass.AnimType.Translation:
							case Animations.AnimationClass.AnimType.TranslationX:
							case Animations.AnimationClass.AnimType.TranslationY:
							case Animations.AnimationClass.AnimType.TranslationZ: {
								var a = _lods[idxLod].Points[selection.SelectedVertices[0].Value];
								var b = _lods[idxLod].Points[selection.SelectedVertices[1].Value];
								var diff = b - a;
								if (AreVectorsAboutEqual(diff, animations.AxisData[idxLod][idxAnimClass][1])) {
									ret.Add(selection.Name);
									break;
								}

								continue;
							}
						}
					}
				}
			}

			break;
		}

		return ret.ToArray();
	}

	public bool AreVectorsAboutEqual(Vector3P a, Vector3P b) {
		if (System.Math.Abs(a.X - b.X) > 0.000001) return false;
		if (System.Math.Abs(a.Y - b.Y) > 0.000001) return false;
		if (System.Math.Abs(a.Z - b.Z) > 0.000001) return false;
		return true;
	}

	public string CombineModelCfg( string[] existing ) {
			var ret = new System.Text.StringBuilder();

			var inSkeletons = false;
			var inModels = false;
			var hasDefault = false;

			foreach ( var line in existing ) {
				if (line.ToLower().Contains( "class cfgskeletons" )) {
					if (ModelInfo.Skeleton.Name != "") inSkeletons = true;
					inModels = false;
				}
				else if (line.ToLower().Contains( "class cfgmodels" )) {
					inSkeletons = false;
					inModels = true;
				}
				else if (inSkeletons) {
					if (line.Contains( "\tclass " + ModelInfo.Skeleton.Name )) inSkeletons = false;
					else if (line.StartsWith( "};" )) {
						ret.AppendLine( "\tclass " + ModelInfo.Skeleton.Name );
						ret.AppendLine( "\t{" );
						ret.AppendLine( "\t\tIsDiscrete = " + ModelInfo.Skeleton.Discrete.GetHashCode() + ";" );
						ret.AppendLine( "\t\tSkeletonInherit = \"" + ModelInfo.Skeleton.PivotsNameObsolete + "\";" );
						ret.AppendLine( "\t\tSkeletonBones[] =" );
						ret.AppendLine( "\t\t{" );
						for (var idxBone = 0; idxBone < ModelInfo.Skeleton.Bones.Length; idxBone += 2) {
							ret.Append( "\t\t\t\"" + ModelInfo.Skeleton.Bones[idxBone] + "\", \"" + ModelInfo.Skeleton.Bones[idxBone + 1] + "\"" );
							if (idxBone + 1 != ModelInfo.Skeleton.Bones.Length - 1) ret.Append(',');
							ret.AppendLine();
						}
						ret.AppendLine( "\t\t};" );
						ret.AppendLine( "\t};" );
					}
				}
				else if (inModels) {
					if (line.Contains( "\tclass " + Path.GetFileNameWithoutExtension( FileName ) )) inModels = false;
					else if (line.Contains( "class Default" )) hasDefault = true;
					else if (line.StartsWith( "};" )) {
						inModels = false;
						if (!hasDefault) {
							ret.AppendLine( "\tclass Default" );
							ret.AppendLine( "\t{" );
							ret.AppendLine( "\t\tSections[] = {};" );
							ret.AppendLine( "\t\tSectionsInherit = \"\";" );
							ret.AppendLine( "\t\tSkeletonName = \"\";" );
							ret.AppendLine( "\t};" );
						}

						ret.AppendLine( "\tclass " + Path.GetFileNameWithoutExtension( FileName ) + ": Default" );
						ret.AppendLine( "\t{" );

						var selections = GetHiddenSelectionNames();
						if (selections.Length != 0) {
							ret.AppendLine( "\t\tSections[] =" );
							ret.AppendLine( "\t\t{" );

							for (var idxSelection = 0; idxSelection < selections.Length; idxSelection++) {
								ret.Append( "\t\t\t\"" + selections[idxSelection] + "\"" );
								if (idxSelection + 1 != selections.Length) ret.Append(',');
								ret.AppendLine();
							}

							ret.AppendLine( "\t\t};" );
						}

						if (ModelInfo.Skeleton.Name != "") ret.AppendLine( "\t\tSkeletonName = \"" + ModelInfo.Skeleton.Name + "\";" );
						if (animations.AnimationClasses != null) {
							var axes = GetAxisSelectionNames();

							ret.AppendLine( "\t\tclass Animations" );
							ret.AppendLine( "\t\t{" );
							for (var idxAnimClass = 0; idxAnimClass < animations.AnimationClasses.Length; idxAnimClass++)
							{
								ret.AppendLine( "\t\t\tclass " + animations.AnimationClasses[idxAnimClass].AnimName );
								ret.AppendLine( "\t\t\t{" );

								ret.AppendLine( "\t\t\t\ttype = \"" + animations.AnimationClasses[idxAnimClass].AnimationType + "\";" );
								ret.AppendLine( "\t\t\t\tsource = \"" + animations.AnimationClasses[idxAnimClass].AnimSource + "\";" );
								ret.AppendLine( "\t\t\t\tselection = \"" + ModelInfo.Skeleton.Bones[animations.Anims2Bones[0][idxAnimClass] * 2] + "\";" );
								if (axes[idxAnimClass] != "") ret.AppendLine( "\t\t\t\taxis = \"" + axes[idxAnimClass] + "\";" );
								if (animations.AnimationClasses[idxAnimClass].SourceAddress != 0)
									ret.AppendLine( "\t\t\t\tsourceAddress = \"" + animations.AnimationClasses[idxAnimClass].SourceAddress + "\";" );
								if (animations.AnimationClasses[idxAnimClass].MinPhase != 0 || animations.AnimationClasses[idxAnimClass].MaxPhase != 1)
									ret.AppendLine( "\t\t\t\tminPhase = \"" + animations.AnimationClasses[idxAnimClass].MinPhase + "\";" );
								if (animations.AnimationClasses[idxAnimClass].MaxPhase != 1)
									ret.AppendLine( "\t\t\t\tmaxPhase = \"" + animations.AnimationClasses[idxAnimClass].MaxPhase + "\";" );
								ret.AppendLine( "\t\t\t\tminValue = \"" + animations.AnimationClasses[idxAnimClass].MinValue + "\";" );
								ret.AppendLine( "\t\t\t\tmaxValue = \"" + animations.AnimationClasses[idxAnimClass].MaxValue + "\";" );
								switch (animations.AnimationClasses[idxAnimClass].AnimationType)
								{
									case Animations.AnimationClass.AnimType.Rotation:
									case Animations.AnimationClass.AnimType.RotationX:
									case Animations.AnimationClass.AnimType.RotationY:
									case Animations.AnimationClass.AnimType.RotationZ:
										ret.AppendLine( "\t\t\t\tangle0 = \"" + animations.AnimationClasses[idxAnimClass].Angle0 + "\";" );
										ret.AppendLine( "\t\t\t\tangle1 = \"" + animations.AnimationClasses[idxAnimClass].Angle1 + "\";" );
										break;
									case Animations.AnimationClass.AnimType.Translation:
									case Animations.AnimationClass.AnimType.TranslationX:
									case Animations.AnimationClass.AnimType.TranslationY:
									case Animations.AnimationClass.AnimType.TranslationZ:
										ret.AppendLine( "\t\t\t\toffset0 = \"" + animations.AnimationClasses[idxAnimClass].Offset0 + "\";" );
										ret.AppendLine( "\t\t\t\toffset1 = \"" + animations.AnimationClasses[idxAnimClass].Offset1 + "\";" );
										break;
									case Animations.AnimationClass.AnimType.Direct:
										ret.AppendLine( "\t\t\t\taxisPos = \"" + animations.AnimationClasses[idxAnimClass].AxisPos + "\";" );
										ret.AppendLine( "\t\t\t\taxisDir = \"" + animations.AnimationClasses[idxAnimClass].AxisDir + "\";" );
										ret.AppendLine( "\t\t\t\tangle = \"" + animations.AnimationClasses[idxAnimClass].Angle + "\";" );
										ret.AppendLine( "\t\t\t\taxisOffset = \"" + animations.AnimationClasses[idxAnimClass].AxisOffset + "\";" );
										break;
									case Animations.AnimationClass.AnimType.Hide:
										ret.AppendLine( "\t\t\t\thideValue = \"" + animations.AnimationClasses[idxAnimClass].HideValue + "\";" );
										break;
								}

								ret.AppendLine( "\t\t\t};" );
							}

							ret.AppendLine( "\t\t};" );
						}

						ret.AppendLine( "\t};" );
					}
				}

				ret.AppendLine( line );
			}

			return ret.ToString();
		}

		public string GetModelCfg() {
		var ret = new System.Text.StringBuilder();

			if (ModelInfo.Skeleton.Bones != null) {
				ret.AppendLine( "class CfgSkeletons" );
				ret.AppendLine( "{" );
				ret.AppendLine( "\tclass " + ModelInfo.Skeleton.Name );
				ret.AppendLine( "\t{" );
				ret.AppendLine( "\t\tIsDiscrete = " + ModelInfo.Skeleton.Discrete.GetHashCode() + ";" );
				ret.AppendLine( "\t\tSkeletonInherit = \"" + ModelInfo.Skeleton.PivotsNameObsolete + "\";" );
				ret.AppendLine( "\t\tSkeletonBones[] =" );
				ret.AppendLine( "\t\t{" );
				for (var idxBone = 0; idxBone < ModelInfo.Skeleton.Bones.Length; idxBone += 2) {
					ret.Append( "\t\t\t\"" + ModelInfo.Skeleton.Bones[idxBone] + "\", \"" + ModelInfo.Skeleton.Bones[idxBone + 1] + "\"" );
					if (idxBone + 1 != ModelInfo.Skeleton.Bones.Length - 1) ret.Append(',');
					ret.AppendLine();
				}
				ret.AppendLine( "\t\t};" );
				ret.AppendLine( "\t};" );
				ret.AppendLine( "};\n" );
			}

			ret.AppendLine( "class CfgModels" );
			ret.AppendLine( "{" );
			ret.AppendLine( "\tclass Default" );
			ret.AppendLine( "\t{" );
			ret.AppendLine( "\t\tSections[] = {};" );
			ret.AppendLine( "\t\tSectionsInherit = \"\";" );
			ret.AppendLine( "\t\tSkeletonName = \"\";" );
			ret.AppendLine( "\t};" );
			ret.AppendLine( "\tclass " + Path.GetFileNameWithoutExtension( FileName ) + ": Default" );
			ret.AppendLine( "\t{" );

			var selections = GetHiddenSelectionNames();
			if (selections.Length != 0) {
				ret.AppendLine( "\t\tSections[] =" );
				ret.AppendLine( "\t\t{" );

				for (var idxSelection = 0; idxSelection < selections.Length; idxSelection++) {
					ret.Append( "\t\t\t\"" + selections[idxSelection] + "\"" );
					if (idxSelection + 1 != selections.Length) ret.Append(',');
					ret.AppendLine();
				}

				ret.AppendLine( "\t\t};" );
			}

			if (ModelInfo.Skeleton.Name != "") ret.AppendLine( "\t\tSkeletonName = \"" + ModelInfo.Skeleton.Name + "\";" );
			if (animations.AnimationClasses != null) {
				var axes = GetAxisSelectionNames();

				ret.AppendLine( "\t\tclass Animations" );
				ret.AppendLine( "\t\t{" );
				for (var idxAnimClass = 0; idxAnimClass < animations.AnimationClasses.Length; idxAnimClass++) {
					ret.AppendLine( "\t\t\tclass " + animations.AnimationClasses[idxAnimClass].AnimName );
					ret.AppendLine( "\t\t\t{" );

					ret.AppendLine( "\t\t\t\ttype = \"" + animations.AnimationClasses[idxAnimClass].AnimationType + "\";" );
					ret.AppendLine( "\t\t\t\tsource = \"" + animations.AnimationClasses[idxAnimClass].AnimSource + "\";" );
					ret.AppendLine( "\t\t\t\tselection = \"" + ModelInfo.Skeleton.Bones[animations.Anims2Bones[0][idxAnimClass]*2] + "\";" );
					if (axes[idxAnimClass] != "")
						ret.AppendLine( "\t\t\t\taxis = \"" + axes[idxAnimClass] + "\";" );
					
					if (animations.AnimationClasses[idxAnimClass].SourceAddress != 0)
						ret.AppendLine( "\t\t\t\tsourceAddress = \"" + animations.AnimationClasses[idxAnimClass].SourceAddress + "\";" );
					if (animations.AnimationClasses[idxAnimClass].MinPhase != 0 || animations.AnimationClasses[idxAnimClass].MaxPhase != 1)
						ret.AppendLine( "\t\t\t\tminPhase = \"" + animations.AnimationClasses[idxAnimClass].MinPhase + "\";" );
					if (animations.AnimationClasses[idxAnimClass].MaxPhase != 1)
						ret.AppendLine( "\t\t\t\tmaxPhase = \"" + animations.AnimationClasses[idxAnimClass].MaxPhase + "\";" );
					ret.AppendLine( "\t\t\t\tminValue = \"" + animations.AnimationClasses[idxAnimClass].MinValue + "\";" );
					ret.AppendLine( "\t\t\t\tmaxValue = \"" + animations.AnimationClasses[idxAnimClass].MaxValue + "\";" );
					switch (animations.AnimationClasses[idxAnimClass].AnimationType) {
						case Animations.AnimationClass.AnimType.Rotation:
						case Animations.AnimationClass.AnimType.RotationX:
						case Animations.AnimationClass.AnimType.RotationY:
						case Animations.AnimationClass.AnimType.RotationZ:
							ret.AppendLine( "\t\t\t\tangle0 = \"" + animations.AnimationClasses[idxAnimClass].Angle0 + "\";" );
							ret.AppendLine( "\t\t\t\tangle1 = \"" + animations.AnimationClasses[idxAnimClass].Angle1 + "\";" );
							break;
						case Animations.AnimationClass.AnimType.Translation:
						case Animations.AnimationClass.AnimType.TranslationX:
						case Animations.AnimationClass.AnimType.TranslationY:
						case Animations.AnimationClass.AnimType.TranslationZ:
							ret.AppendLine( "\t\t\t\toffset0 = \"" + animations.AnimationClasses[idxAnimClass].Offset0 + "\";" );
							ret.AppendLine( "\t\t\t\toffset1 = \"" + animations.AnimationClasses[idxAnimClass].Offset1 + "\";" );
							break;
						case Animations.AnimationClass.AnimType.Direct:
							ret.AppendLine( "\t\t\t\taxisPos = \"" + animations.AnimationClasses[idxAnimClass].AxisPos + "\";" );
							ret.AppendLine( "\t\t\t\taxisDir = \"" + animations.AnimationClasses[idxAnimClass].AxisDir + "\";" );
							ret.AppendLine( "\t\t\t\tangle = \"" + animations.AnimationClasses[idxAnimClass].Angle + "\";" );
							ret.AppendLine( "\t\t\t\taxisOffset = \"" + animations.AnimationClasses[idxAnimClass].AxisOffset + "\";" );
							break;
						case Animations.AnimationClass.AnimType.Hide:
							ret.AppendLine( "\t\t\t\thideValue = \"" + animations.AnimationClasses[idxAnimClass].HideValue + "\";" );
							break;
					}

					ret.AppendLine( "\t\t\t};" );
				}

				ret.AppendLine( "\t\t};" );
			}
			
			ret.AppendLine( "\t};" );
			ret.AppendLine( "};" );

			return ret.ToString();
		}


}