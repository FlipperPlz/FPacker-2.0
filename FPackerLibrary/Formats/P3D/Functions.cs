namespace FPacker.P3D; 

public static class Functions {
    public static void Swap<T>(ref T v1, ref T v2) => (v1, v2) = (v2, v1);
    public static bool EqualsFloat(float f1, float f2, float tolerance = 0.0001f) => System.Math.Abs(f1 - f2) <= tolerance;
    internal static IEnumerable<T> Yield<T>(this T src) { yield return src; }
    public static IEnumerable<T> Yield<T>(params T[] elems) => elems;
    public static string CharsToString(this IEnumerable<char> chars) => new(chars.ToArray());
}