using Antlr4.Runtime;

namespace FPackerLibrary.Formats.Enforce; 

public interface IEnforceDeserializable<in Tctx, out Tself> : IEnforceSerializable where Tctx : ParserRuleContext where Tself : IEnforceSerializable {
    public Tself FromEnforceContext(Tctx ctx);
}