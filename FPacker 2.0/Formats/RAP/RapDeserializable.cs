using Antlr4.Runtime;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public interface IRapDeserializable : IRapSerializable {
    public Tself FromRapContext<Tself>(ParserRuleContext ctx) where Tself : IRapDeserializable;
    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) where Tself : IRapDeserializable;
}

public interface IRapSerializable {
    public string ToRapFormat();
    public void ToBinaryContext(RapBinaryWriter writer);
}

