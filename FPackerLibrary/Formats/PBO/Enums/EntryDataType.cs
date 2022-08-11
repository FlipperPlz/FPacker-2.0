using System.Diagnostics.CodeAnalysis;

namespace FPackerLibrary.PBO.Enums; 

[SuppressMessage("ReSharper", "InconsistentNaming")] //Acronym: RVMAT
public enum EntryDataType {
    Obfuscated,
    EnforceScript,
    Model,
    Texture,
    PoseidonConfig,
    RVMAT, //Follows same syntax as PoseidonConfig
    StringTable,
    Asset,
    Audio,
    GuiLayout,
    HtmlGui,
    Unknown,
    UnknownBinarized,
    BinarizedRVMAT,
    BinarizedPoseidonConfig,
    BinarizedTexHeaders
}