using Antlr4.Runtime;

namespace FPacker.Formats.RAP.Models; 

public interface IRapDeserializable<in Tctx, out Tself> : IRapSerializable where Tctx : ParserRuleContext where Tself : IRapSerializable {
    public Tself FromRapContext(Tctx ctx);
}