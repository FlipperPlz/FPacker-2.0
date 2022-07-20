using System.Text;
using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models;

public class RapVariableStatement : IRapEntry {
    public string VariableName { get; set; }
    public RapValueType VariableType { get; private set; }
    public IRapSerializable VariableValue { get; set; }

    public string ToRapFormat() {
        var builder = new StringBuilder(VariableName);
        if (VariableValue is RapArray) builder.Append("[]");
        return builder.Append(" = ").Append(VariableValue.ToRapFormat()).Append(';').ToString();
    }

    public void ToBinaryContext(RapBinaryWriter writer) {
        throw new NotImplementedException();
    }

    public Tself FromRapContext<Tself>(ParserRuleContext context) where Tself : IRapDeserializable {
        if (context is not PoseidonParser.VariableAssignmentContext ctx) throw new Exception();
        if (ctx.variableInitializer().arrayInitializer() is { } arrayInitializer) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            VariableValue = new RapArray().FromRapContext<RapArray>(arrayInitializer);
            return (Tself) (IRapEntry) this;
        }
        if (ctx.variableInitializer().literal().literalFloat() is { } floatContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            VariableValue = new RapFloat().FromRapContext<RapFloat>(floatContext);
            return (Tself) (IRapEntry) this;
        }
        if (ctx.variableInitializer().literal().literalInteger() is { } integerContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            VariableValue = new RapInt().FromRapContext<RapInt>(integerContext);
            return (Tself) (IRapEntry) this;
        }
        
        if (ctx.variableInitializer().literal().literalString() is { } stringContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            VariableValue = new RapString().FromRapContext<RapString>(stringContext);
            return (Tself) (IRapEntry) this;
        }

        throw new Exception($"No value was found for, {ctx.variableDeclaratorId().identifier().GetText()}");
    }

    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool isArray = false) where Tself : IRapDeserializable {
        if (isArray) {
            VariableType = RapValueType.Array;
            VariableName = reader.ReadAsciiZ();
            VariableValue = reader.ReadBinarizedValue<RapArray>();
            return (Tself) (IRapEntry) this;
        }

        VariableType = (RapValueType) reader.ReadByte();
        VariableName = reader.ReadAsciiZ();
        
        IRapSerializable value = VariableType switch {
            RapValueType.String => reader.ReadBinarizedValue<RapString>(),
            RapValueType.Variable => throw new Exception("Variables not supported yet."),
            RapValueType.Float => reader.ReadBinarizedValue<RapFloat>(),
            RapValueType.Long => reader.ReadBinarizedValue<RapInt>(),
            RapValueType.Array => reader.ReadBinarizedValue<RapArray>(),
            _ => throw new Exception("How did we get here?")
        };
        VariableValue = value;
        return (Tself) (IRapEntry) this;
    }
}