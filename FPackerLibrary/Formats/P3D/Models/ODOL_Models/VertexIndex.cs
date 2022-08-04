namespace FPackerLibrary.P3D.Models.ODOL; 

public struct VertexIndex
{
    public int Value;

    public static implicit operator int(VertexIndex vi) => vi.Value;

    public static implicit operator VertexIndex(ushort vi) => new() {Value = ((vi == ushort.MaxValue) ? -1 : vi) };

    public static implicit operator VertexIndex(int vi) => new() {
        Value = vi
    };
}