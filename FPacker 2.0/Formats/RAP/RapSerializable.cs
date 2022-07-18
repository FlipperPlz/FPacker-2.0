using Antlr4.Runtime;

namespace FPacker.Formats.RAP.Models; 

public interface RapSerializable {
    public string ToRapFormat();
}