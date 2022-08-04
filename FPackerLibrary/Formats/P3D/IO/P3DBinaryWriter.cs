using System.IO.Compression;
using System.Text;
using FPackerLibrary.Compression;

namespace FPackerLibrary.P3D.IO;

public class P3DBinaryWriter : BinaryWriter {
    public bool UseCompressionFlag { get; set; }
    public bool UseLZOCompression { get; set; }
    public uint Version { get; set; }


    public long Position {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    public P3DBinaryWriter(Stream dstStream) : base(dstStream, Encoding.ASCII) { }
    public P3DBinaryWriter(Stream dstStream, bool leaveOpen) : base(dstStream, Encoding.ASCII, leaveOpen) { }

    public void WriteAscii(string text, uint len) {
        Write(text.ToCharArray());
        var num = (uint) (len - (ulong) (text.Length));
        var num2 = 0;
        while (num2 < (num)) {
            Write('\0');
            num2++;
        }
    }

    public void WriteAscii32(string text) {
        Write(text.Length);
        Write(text.ToCharArray());
    }

    public void WriteUInt24(uint length) {
        Write((byte) (length & 0xFF));
        Write((byte) ((length >> 8) & 0xFF));
        Write((byte) ((length >> 16) & 0xFF));
    }

    private void WriteArrayBase<T>(IEnumerable<T> array, Action<P3DBinaryWriter, T> write) {
        foreach (var item in array) write(this, item);
    }
    
    public void WriteCompressed(byte[] data) {

        if (UseLZOCompression) WriteLZO(data, false);
        else this.WriteLZSS(data, false);
    }

    private void WriteCondensedArrayBase<T>(IReadOnlyCollection<T> array, Action<P3DBinaryWriter, T> write, int sizeOfT) {
        WriteInt32(array.Count);
        if(array.Count == 0) Write(false);
        else {
            Write(true);
            foreach (var val in array) write(this, val);
            return;
        }

        var expectedSize = (uint) (array.Count * sizeOfT);
        WriteCompressed(BitConverter.GetBytes(expectedSize));
        WriteArrayBase(array, write);
    }

    public void WriteArray<T>(T[] array, Action<P3DBinaryWriter, T> write) {
        Write(array.Length);
        WriteArrayBase(array, write);
    }
    
    public void WriteAsciiZ(string text) {
        Write(text.ToCharArray());
        Write(char.MinValue);
    }
    public void WriteCondensedIntArray(float[] array) => WriteCondensedArrayBase(array, static (w, v) => w.Write(v), 4);

    public void WriteCondensedObjectArray<T>(IReadOnlyCollection<T> objs, int sizeOfT) where T : IDeserializable, new() {
        WriteCondensedArrayBase(objs, static (w, v) => v.WriteObject(w), sizeOfT);
    } 
    
    public void WriteCompressedFloatArray(float[] array) => WriteCompressedArray(array, static (w, v) => w.WriteSingle(v), 4);
    public void WriteCompressedIntArray(int[] array) => WriteCompressedArray(array, static (w, v) => w.WriteInt32(v), 4);

    public void WriteFloats(IEnumerable<float> elements) => WriteArrayBase<float>(elements, static (r, e) => r.Write(e));

    public void WriteUshorts(IEnumerable<ushort> elements) => WriteArrayBase<ushort>(elements, static (r,e) => r.Write(e));

    private void WriteCompressed(byte[] bytes, bool forceCompressed = false) {
        if (UseLZOCompression) WriteLZO(bytes, forceCompressed);
        else WriteLZSS(bytes);
    }

    public void WriteCompressedArray<T>(T[] array, Action<P3DBinaryWriter, T> write, int size) {
        throw new NotImplementedException("Broken");
        var mem = new MemoryStream();
        using (var writer = new P3DBinaryWriter(mem)) foreach (var item in array)write(writer, item);
        Write(array.Length);
        var bytes = mem.ToArray();
        if (array.Length * size != bytes.Length) throw new InvalidOperationException();
    }
    
    public void WriteLZSS(byte[] bytes, bool inPAA = false) {
        if (bytes.Length < 1024 && !inPAA) {
            Write(bytes);
        } else {
            var cSum = inPAA ? bytes.Sum(static e => (sbyte)e) : bytes.Sum(static e => e);
            using (var lzss =  new LzssStream(BaseStream, System.IO.Compression.CompressionMode.Compress, true)) {
                lzss.Write(bytes, 0, bytes.Length);
            }
            Write(BitConverter.GetBytes(cSum));
        }
    }
    
    public void WriteLZO(byte[] bytes, bool forceCompressed = false) {
        if (bytes.Length < 1024 && !forceCompressed) {
            if (UseCompressionFlag) Write((byte)2);
            Write(bytes);
        } else {
            if (UseCompressionFlag) Write((byte)2);
            Write(MiniLZO.MiniLZO.Compress(bytes));
        }
    }


    public void WriteUInt32(uint uint32) => Write(BitConverter.GetBytes(uint32));

    public void WriteSingle(float single) => Write(BitConverter.GetBytes(single));

    public void WriteInt32(int i) => Write(BitConverter.GetBytes(i));
}
