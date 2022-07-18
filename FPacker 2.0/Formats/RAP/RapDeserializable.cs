using Antlr4.Runtime;

namespace FPacker.Formats.RAP.Models; 

public interface RapDeserializable<in Tctx, out Tself> where Tctx : ParserRuleContext where Tself : RapSerializable {
    public Tself FromRapContext(Tctx ctx);
}