using System.Globalization;
using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Math;

public struct ColorP {
	public float Red { get; private set; }
	public float Green { get; private set; }
	public float Blue { get; private set; }
	public float Alpha { get; private set; }
	
	public ColorP(float r, float g, float b, float a) {
		this.Red = r;
		this.Green = g;
		this.Blue = b;
		this.Alpha = a;
	}

	public ColorP(P3DBinaryReader input) {
		this.Red = input.ReadSingle();
		this.Green = input.ReadSingle();
		this.Blue = input.ReadSingle();
		this.Alpha = input.ReadSingle();
	}

	public void Read(P3DBinaryReader input) {
		this.Red = input.ReadSingle();
		this.Green = input.ReadSingle();
		this.Blue = input.ReadSingle();
		this.Alpha = input.ReadSingle();
	}

	public readonly void Write(P3DBinaryWriter output) {
		output.Write(this.Red);
		output.Write(this.Green);
		output.Write(this.Blue);
		output.Write(this.Alpha);
	}

	public readonly override string ToString() {
		var cultureInfo = new CultureInfo("en-GB");
		return string.Concat(new string[] {
			"{",
			this.Red.ToString(cultureInfo.NumberFormat),
			",",
			this.Green.ToString(cultureInfo.NumberFormat),
			",",
			this.Blue.ToString(cultureInfo.NumberFormat),
			",",
			this.Alpha.ToString(cultureInfo.NumberFormat),
			"}"
		});
	}
}