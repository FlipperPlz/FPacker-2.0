using Antlr4.Runtime;

namespace FPacker.Formats.RAP.Models; 

public interface IRapSerializable {
    public string ToRapFormat();
}