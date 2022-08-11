using System.Text;
using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.PBO.Enums;

namespace FPackerLibrary.PBO.Models; 

public class PBOConfigEntry : PBOEntry {
    private readonly RapFile _cfgObject;
    public bool BinarizeEntry { get; set; }

    public PBOConfigEntry(RapFile cfgObject, string pboPath = "", bool binarize = true) : base(pboPath, (int) EntryPackingType.Uncompressed, EntryDataType.PoseidonConfig, Encoding.UTF8.GetBytes(cfgObject.ToRapFormat())) {
        _cfgObject = cfgObject;
        if (binarize) Binarize();
        else this.DecompressEntry();
    }

    public void Binarize() {
        BinarizeEntry = true;
        var fileName = Guid.NewGuid().ToString();
        var filePath = $"{Path.GetTempPath()}{fileName}.cpp";
        var binnedFilePath = $"{Path.GetTempPath()}{fileName}.bin";
        File.WriteAllBytes(filePath, Data);
        var binarizedData = PBOUtilities.BinarizeConfig(filePath);
        File.Delete(binnedFilePath);
        ChangeData(binarizedData, EntryPackingType.Uncompressed);
        PBOPath = PBOPath.Replace(".cpp", ".bin");
    }

    public void DeBinarize() {
        BinarizeEntry = false;
        Data = Encoding.UTF8.GetBytes(_cfgObject.ToRapFormat());
        OriginalDataSize = Data.LongLength;
        PackedDataSize = Data.LongLength;
        PBOPath = PBOPath.Replace(".bin", ".cpp");
    }
    
}