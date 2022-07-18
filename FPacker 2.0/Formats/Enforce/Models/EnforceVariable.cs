using FPacker.Antlr.Enforce;
using FPacker.Formats.RAP.Models;

namespace FPacker.Formats.Enforce.Models; 

public class EnforceLocalVariable : EnforceDeserializable<EnforceParser.LocalVariableDeclarationContext, EnforceLocalVariable>, EnforceSerializable {
    public string VariableName { get; set; }
    public string SpecifiedVariableType; 

    public EnforceLocalVariable FromEnforceContext(EnforceParser.LocalVariableDeclarationContext ctx) {
        throw new NotImplementedException();
    }

    public string ToEnforceFormat() {
        throw new NotImplementedException();
    }
}