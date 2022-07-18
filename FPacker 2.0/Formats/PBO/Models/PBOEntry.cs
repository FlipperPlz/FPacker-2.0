using System.Diagnostics.CodeAnalysis;
using FPacker.Compression;
using FPacker.PBO.Enums;

namespace FPacker.PBO.Models; 

[SuppressMessage("ReSharper", "MemberCanBeInternal")]
public class PBOEntry {
    private static readonly string[] CompressionAutoExcludes = new[] {"wss", "ogg", "jpg", "jpeg", "wav", "fsm", "paa"};
    
    public string PBOPath { get; set; }
    public string ObfuscatedPBOPath { get; set; } = "";
    public int MimeType;

    public byte[] Data { get; protected set; }
    public long OriginalDataSize { get; protected set; }
    public long PackedDataSize { get; protected set; }
    public ulong Offset { get; set; }
    public ulong Timestamp { get; set; }
    private readonly string SystemPath;
    private readonly EntryDataType EntryDataType;
    
    public PBOEntry(string pboPath, int packingType, EntryDataType fileType, byte[] entryData, string systemPath = "") {
        SystemPath = systemPath;
        EntryDataType = fileType;
        PBOPath = pboPath;
        MimeType = packingType;
        //MimeType = packingType == (int) EntryPackingType.Compressed
                   // && !CompressionAutoExcludes.Contains(Path.GetExtension(SystemPath))
                   // && EntryDataType is not (EntryDataType.PoseidonConfig and EntryDataType.EnforceScript) 
            // ? (int) EntryPackingType.Uncompressed : packingType;
        Data = (packingType == (int) EntryPackingType.Compressed) ? BohemiaLZSS.Compress(entryData) : entryData;
        OriginalDataSize = entryData.LongLength;
        PackedDataSize = Data.LongLength;
        Timestamp = Convert.ToUInt64(DateTime.UtcNow.Subtract(new DateTime(1920, 1, 1)).TotalSeconds);
        Offset = 0;
    }

    public void ChangeData(IEnumerable<byte> newData, EntryPackingType? packingType = null) {
        var data = newData.ToArray();
        switch (packingType) {
            case EntryPackingType.Compressed:
                OriginalDataSize = data.LongLength;
                Data = BohemiaLZSS.Compress(data);
                PackedDataSize = Data.LongLength;
                Timestamp = Convert.ToUInt64(DateTime.UtcNow.Subtract(new DateTime(1920, 1, 1)).TotalSeconds);
                MimeType = ((int) packingType)!;
                break;
            case EntryPackingType.Uncompressed:
                Data = data;
                OriginalDataSize = data.LongLength;
                PackedDataSize = data.LongLength;
                Timestamp = Convert.ToUInt64(DateTime.UtcNow.Subtract(new DateTime(1920, 1, 1)).TotalSeconds);
                MimeType = ((int) packingType)!;
                break;
            case null: 
                OriginalDataSize = data.LongLength;
                Data = (MimeType == (int) EntryPackingType.Compressed) ? BohemiaLZSS.Compress(data) : data;
                break;
            case EntryPackingType.Encrypted: break;
        }
    }
    
    public void DecompressEntry() {
        if(!IsCompressed()) return;
        Data = BohemiaLZSS.Decompress(Data, OriginalDataSize);
        PackedDataSize = OriginalDataSize;
        MimeType = (int) EntryPackingType.Uncompressed;
    }

    public void CompressEntry() {
        if(IsCompressed()) return;
        OriginalDataSize = Data.LongLength;
        Data = BohemiaLZSS.Compress(Data);
        PackedDataSize = Data.LongLength;
        MimeType = (int) EntryPackingType.Compressed;
    }

    public bool IsCompressed() => MimeType == (int) EntryPackingType.Compressed;
    
}

