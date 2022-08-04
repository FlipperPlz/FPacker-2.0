using Antlr4.Runtime;

namespace FPacker.Formats.Enforce; 

public interface IEnforceDeserializable<in Tctx, out Tself> : IEnforceSerializable where Tctx : ParserRuleContext where Tself : IEnforceSerializable {
    public Tself FromEnforceContext(Tctx ctx);
}