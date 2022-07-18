using Antlr4.Runtime;

namespace FPacker.Formats.Enforce; 

public interface EnforceDeserializable<in Tctx, out Tself> where Tctx : ParserRuleContext where Tself : EnforceSerializable {
    public Tself FromEnforceContext(Tctx ctx);
}