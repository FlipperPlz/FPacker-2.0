namespace FPacker.P3D.Math;

public class Quaternion {
	public float X => this.x;
	public float Y => this.x;
	public float Z => this.x;
	public float W => this.x;

	public Quaternion Inverse {
		get {
			this.normalize();
			return this.Conjugate;
		}
	}
	
	public Quaternion Conjugate => new Quaternion(0f - this.x, 0f - this.y, 0f - this.z, this.w);

	// Token: 0x06000207 RID: 519 RVA: 0x0000A000 File Offset: 0x00008200
	public static Quaternion readCompressed(byte[] data) {
		var num = (float) ((double) (-(double) BitConverter.ToInt16(data, 0)) / 16384.0);
		var num2 = (float) ((double) BitConverter.ToInt16(data, 2) / 16384.0);
		var num3 = (float) ((double) (-(double) BitConverter.ToInt16(data, 4)) / 16384.0);
		var num4 = (float) ((double) BitConverter.ToInt16(data, 6) / 16384.0);
		return new Quaternion(num, num2, num3, num4);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000A06C File Offset: 0x0000826C
	public Quaternion() {
		this.w = 1f;
		this.x = 0f;
		this.y = 0f;
		this.z = 0f;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000A0A0 File Offset: 0x000082A0
	public Quaternion(float x, float y, float z, float w) {
		this.w = w;
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000A0C8 File Offset: 0x000082C8
	public static Quaternion operator *(Quaternion a, Quaternion b) {
		float num = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
		float num2 = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
		float num3 = a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x;
		float num4 = a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w;
		return new Quaternion(num2, num3, num4, num);
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000A1C0 File Offset: 0x000083C0
	public void normalize() {
		float num = (float) (1.0 /
		                     System.Math.Sqrt((double) (this.x * this.x + this.y * this.y + this.z * this.z +
		                                                this.w * this.w)));
		this.x *= num;
		this.y *= num;
		this.z *= num;
		this.w *= num;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000A254 File Offset: 0x00008454
	public Vector3P transform(Vector3P xyz) {
		Quaternion b = new Quaternion(xyz.X, xyz.Y, xyz.Z, 0f);
		Quaternion quaternion = this * b * this.Inverse;
		return new Vector3P(quaternion.x, quaternion.y, quaternion.z);
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000A2AC File Offset: 0x000084AC
	public float[,] asRotationMatrix() {
		float[,] array = new float[3, 3];
		double num = (double) (this.x * this.y);
		double num2 = (double) (this.w * this.z);
		double num3 = (double) (this.w * this.x);
		double num4 = (double) (this.w * this.y);
		double num5 = (double) (this.x * this.z);
		double num6 = (double) (this.y * this.z);
		double num7 = (double) (this.z * this.z);
		double num8 = (double) (this.y * this.y);
		double num9 = (double) (this.x * this.x);
		array[0, 0] = (float) (1.0 - 2.0 * (num8 + num7));
		array[0, 1] = (float) (2.0 * (num - num2));
		array[0, 2] = (float) (2.0 * (num5 + num4));
		array[1, 0] = (float) (2.0 * (num + num2));
		array[1, 1] = (float) (1.0 - 2.0 * (num9 + num7));
		array[1, 2] = (float) (2.0 * (num6 - num3));
		array[2, 0] = (float) (2.0 * (num5 - num4));
		array[2, 1] = (float) (2.0 * (num6 + num3));
		array[2, 2] = (float) (1.0 - 2.0 * (num9 + num8));
		return array;
	}

	// Token: 0x040001C9 RID: 457
	private float x;

	// Token: 0x040001CA RID: 458
	private float y;

	// Token: 0x040001CB RID: 459
	private float z;

	// Token: 0x040001CC RID: 460
	private float w;
}