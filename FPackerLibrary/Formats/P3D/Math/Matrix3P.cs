using FPackerLibrary.P3D.IO;

namespace FPackerLibrary.P3D.Math;

public class Matrix3P {
	private Vector3P[] _columns;

	public Vector3P Aside => this._columns[0];
	public Vector3P Up => this._columns[1];
	public Vector3P Dir => this._columns[2];
	public Vector3P this[int col] => this._columns[col];

	public float this[int row, int col] {
		get => this[col][row];
		set => this[col][row] = value;
	}
	public Matrix3P() : this(0f) { }

	public Matrix3P(float val) : this(new Vector3P(val), new Vector3P(val), new Vector3P(val)) { }

	public Matrix3P(P3DBinaryReader input) : this(new Vector3P(input), new Vector3P(input), new Vector3P(input)) { }

	private Matrix3P(Vector3P aside, Vector3P up, Vector3P dir) => _columns = new[] {
		aside,
		up,
		dir
	};

	public static Matrix3P operator -(Matrix3P a) => new(-a.Aside, -a.Up, -a.Dir);

	public static Matrix3P operator *(Matrix3P a, Matrix3P b) {
		var matrix3P = new Matrix3P();
		var num = b[0, 0];
		var num2 = b[1, 0];
		var num3 = b[2, 0];
		matrix3P[0, 0] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 0] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 0] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 1];
		num2 = b[1, 1];
		num3 = b[2, 1];
		matrix3P[0, 1] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 1] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 1] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 2];
		num2 = b[1, 2];
		num3 = b[2, 2];
		matrix3P[0, 2] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 2] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 2] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		return matrix3P;
	}

	public void SetTilde(Vector3P a) {
		this.Aside.Y = 0f - a.Z;
		this.Aside.Z = a.Y;
		this.Up.X = a.Z;
		this.Up.Z = 0f - a.X;
		this.Dir.X = 0f - a.Y;
		this.Dir.Y = a.X;
	}

	public void WriteObject(P3DBinaryWriter output) {
		this.Aside.WriteObject(output);
		this.Up.WriteObject(output);
		this.Dir.WriteObject(output);
	}
}