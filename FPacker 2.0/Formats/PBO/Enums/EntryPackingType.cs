namespace FPacker.PBO.Enums; 

public enum EntryPackingType {
    Uncompressed = 0x00000000,
    Compressed = 0x43707273,
    Encrypted = 0x456e6372 
}