using System.Runtime.InteropServices;
using System.Text;
using FPacker.Compression;

namespace FPacker.P3D.IO; 

public class P3DBinaryReader : BinaryReader {
    public bool UseCompressionFlag { get; set; }
    public bool UseLZOCompression { get; set; }
    public int Version { get; set; }
    public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
    
    public P3DBinaryReader( Stream stream ) : base(stream) => UseCompressionFlag = false;
    
    public uint ReadUInt24() => (uint)((int)ReadByte() + ((int)ReadByte() << 8) + ((int)ReadByte() << 16));
    
    public string ReadAscii(int count) {
        var builder = new StringBuilder();
        for (var i = 0; i < count; i++) builder.Append(((char)ReadByte()));
        return builder.ToString();
    }
    
    public string ReadAsciiZ() {
        var builder = new StringBuilder();
        char c;
        while ((c = (char)ReadByte()) != '\0') builder.Append(c);
        return builder.ToString();
    }
    
    private T[] ReadArrayBase<T>(Func<P3DBinaryReader, T> readElement, int size) {
        T[] result;
        try {
            var array = new T[size];
            for (var i = 0; i < size; i++) {
                try {
                    array[i] = readElement(this);
                } catch {
                }
            }
            result = array;
        }
        catch {
            Console.WriteLine("Error!");
            result = null;
        }
        return result;
    }
    
    private T ReadObject<T>() where T : IDeserializable, new() {
        var result = Activator.CreateInstance<T>();
        result.ReadObject(this);
        return result;
    }
    
    public T[] ReadArray<T>(Func<P3DBinaryReader, T> readElement) => ReadArrayBase(readElement, ReadInt32());
		
    public T[] ReadArray<T>() where T : IDeserializable, new() => ReadArray(static (i) => i.ReadObject<T>());
		
    public float[] ReadFloatArray() => ReadArray(static (i) => i.ReadSingle());
		
    public int[] ReadIntArray() => ReadArray(static (i) => i.ReadInt32());
		
    public string[] ReadStringArray() => ReadArray(static (i) => i.ReadAsciiZ());
    
    public T[] ReadCompressedArray<T>(Func<P3DBinaryReader, T> readElement, int elemSize) {
        var num = ReadInt32();
        var expectedSize = (uint)(num * elemSize);
        return new P3DBinaryReader(new MemoryStream(this.ReadCompressed(expectedSize))).ReadArrayBase(readElement, num);
    }
		
    public T[] ReadCompressedArray<T>(Func<P3DBinaryReader, T> readElement) => 
        ReadCompressedArray(readElement, Marshal.SizeOf(typeof(T)));
		
    public T[] ReadCompressedObjectArray<T>(int sizeOfT) where T : IDeserializable, new() => 
        ReadCompressedArray(static (i) => i.ReadObject<T>(), sizeOfT);
    
    public short[] ReadCompressedShortArray() => ReadCompressedArray(static (i) => i.ReadInt16());
		
    public int[] ReadCompressedIntArray() => ReadCompressedArray(static (i) => i.ReadInt32());
		
    public float[] ReadCompressedFloatArray() => ReadCompressedArray(static (i) => i.ReadSingle());
    
    public T[] ReadCondensedArray<T>(Func<P3DBinaryReader, T> readElement, int sizeOfT) {
        var num = ReadInt32();
        var array = new T[num];
        if (ReadBoolean()) {
            var t = readElement(this);
            for (var i = 0; i < num; i++) array[i] = t;
            return array;
        }
        var expectedSize = (uint)(num * sizeOfT);
        var binaryReaderEx = new P3DBinaryReader(new MemoryStream(this.ReadCompressed(expectedSize)));
        array = binaryReaderEx.ReadArrayBase(readElement, num);
        binaryReaderEx.Close();
        return array;
    }
    
    public T[] ReadCondensedObjectArray<T>(int sizeOfT) where T : IDeserializable, new() => 
        ReadCondensedArray(static (i) => i.ReadObject<T>(), sizeOfT);
    
		
    public int[] ReadCondensedIntArray() => ReadCondensedArray(static (i) => i.ReadInt32(), 4);

    public int ReadCompactInteger() {
        var num = (int) ReadByte();
        if ((num & 128) == 0) return num;
        var num2 = (int) ReadByte();
        num += (num2 - 1) * 128;
        return num;
    }
    
    public byte[] ReadCompressed(uint expectedSize) {
        if (expectedSize == 0U) return Array.Empty<byte>();
        return UseLZOCompression ? ReadLZO(expectedSize) : this.ReadLZSS(expectedSize, false);
    }
    
    public byte[] ReadLZO(uint expectedSize) {
        var flag = expectedSize >= 1024U;
        if (UseCompressionFlag) flag = ReadBoolean();
        return !flag ? ReadBytes((int)expectedSize) : BohemiaLZO.readLZO(BaseStream, expectedSize);
    }
    
    public byte[] ReadLZSS(uint expectedSize, bool inPAA = false) {
        if (expectedSize < 1024U && !inPAA) return this.ReadBytes((int)expectedSize);
        var result = new byte[expectedSize];
        BohemiaLZSS.DecompressStream(this.BaseStream, out result, expectedSize, inPAA);
        return result;
    }
    
    public byte[] ReadCompressedIndices(int bytesToRead, uint expectedSize) {
        var array = new byte[expectedSize];
        var num = 0;
        for (var i = 0; i < bytesToRead; i++) {
            var b = ReadByte();
            if ((b & 128) != 0) {
                var b2 = (byte)(b - 127);
                var b3 = ReadByte();
                for (var j = 0; j < b2; j++) array[num++] = b3;
            } else for (var k = 0; k < (b + 1); k++) array[num++] = ReadByte();
        }
        return array;
    }
    
    public uint SkipGridCompressed() {
        var position = Position;
        var num = ReadUInt16();
        for (var i = 0; i < 16; i++) {
            if ((num & 1) == 1) SkipGridCompressed();
            else Position += 4L;
            num = (ushort)(num >> 1);
        }
        return (uint)(Position - position);
    }
}

public interface IDeserializable {
    void ReadObject(P3DBinaryReader input);
    void WriteObject(P3DBinaryWriter output);

}