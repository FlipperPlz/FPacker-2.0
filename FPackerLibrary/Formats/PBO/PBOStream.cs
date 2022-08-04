using System.Security.Cryptography;
using System.Text;
using FPackerLibrary.PBO.Models;

namespace FPackerLibrary.PBO; 

public class PBOStream {
    private readonly FileStream _stream;

    public PBOStream(FileStream stream) {
        _stream = stream;
    }

    public bool WritePBO(IEnumerable<PBOEntry> entries) {
        try {
            var pboEntries = entries.ToList();

            WritePBOFileHeader();
            //pboStream.WriteHeader("prefix", pboPrefix);
            WriteByte(0x0);
            WriteEntryMeta(pboEntries);
            WriteDataSeparator();
            WriteEntryData(pboEntries);
            WritePBOChecksum();
            CloseStream();
            
            return true;
        } catch (Exception e) {
            Console.WriteLine("PBO Exploded :(");
            Console.WriteLine(e.Message);
            return false;
        }
    }

    private void CloseStream() => _stream.Close();
    private void WritePBOFileHeader() {
        WriteByte(0x0);
        WriteString("sreV");
        WriteBytes(new byte[15]);
    }
    
    private void WriteString(string str) {
        var content = Encoding.UTF8.GetBytes(str + "\0");
        _stream.Write(content, 0, content.Length);
    }
    
    private void WriteInt(int value) => _stream.Write(BitConverter.GetBytes(value), 0, 4);
    
    private void WriteUnsignedLong(ulong value) => _stream.Write(BitConverter.GetBytes(value), 0, 4);
    
    private void WriteByte(byte value) => _stream.WriteByte(value);
    
    private void WriteBytes(byte[] values, int length = 0) {
        if (length != 0) {
            _stream.Write(values, 0, length);
            return;
        }
        _stream.Write(values, 0, values.Length);
    }
    
    private void WriteHeader(string key, string value) => WriteString($"{key}\0{value}");

    private void WriteEntryMeta(IEnumerable<PBOEntry> pboEntries, bool obfuscatedPaths = false) {
        foreach (var entry in pboEntries) {
            WriteString(obfuscatedPaths ? entry.ObfuscatedPBOPath : entry.PBOPath);
            WriteInt(entry.MimeType);
            WriteUnsignedLong((ulong) entry.OriginalDataSize);
            WriteUnsignedLong(entry.Offset);
            WriteUnsignedLong(entry.Timestamp);
            WriteUnsignedLong((ulong) entry.PackedDataSize);
        }
    }

    private void WriteEntryData(IEnumerable<PBOEntry> pboEntries) {
        foreach (var entry in pboEntries) WriteBytes(entry.Data);        
    }

    private void WritePBOChecksum() {
        var checksum = CalculatePBOChecksum(_stream);
        WriteByte(0x0);
        WriteBytes(checksum, 20);
    }

    private void WriteDataSeparator() {
        WriteString("");
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
    }
    
    private static byte[] CalculatePBOChecksum(Stream stream) {
        var oldPos = stream.Position;

        stream.Position = 0;
        #pragma warning disable CS0618
        var hash = new SHA1Managed().ComputeHash(stream);
        #pragma warning restore CS0618

        stream.Position = oldPos;

        return hash;
    }
}