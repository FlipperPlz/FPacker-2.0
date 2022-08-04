using FPacker.PBO.Enums;

namespace FPacker.PBO.Models; 

public class PBOEnforceEntry : PBOEntry {
    
    public PBOEnforceEntry(string pboPath, int packingType, EntryDataType fileType, byte[] entryData, string systemPath = "") : base(pboPath, packingType, fileType, entryData, systemPath) { }
}