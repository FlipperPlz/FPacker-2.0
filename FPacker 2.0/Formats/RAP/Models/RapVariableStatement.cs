using System.Text;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models;

public class RapVariableStatement : IRapDeserializable<PoseidonParser.VariableAssignmentContext, RapVariableStatement>, IRapBinarizable<RapVariableStatement> {
    public string VariableName { get; set; }
    public RapValueType VariableType { get; private set; }
    public IRapSerializable VariableValue { get; set; }

    public string ToRapFormat() {
        var builder = new StringBuilder(VariableName);
        if (VariableValue is RapArray) builder.Append("[]");
        return builder.Append(" = ").Append(VariableValue.ToRapFormat()).Append(';').ToString();
    }

    public static RapVariableStatement FromRapFormat(PoseidonParser.VariableAssignmentContext ctx) {
        if (ctx.variableInitializer().arrayInitializer() is { } arrayInitializer) {
            return new RapVariableStatement {
                VariableName = ctx.variableDeclaratorId().identifier().GetText(),
                VariableValue = RapArray.FromParseContext(arrayInitializer)
            };
        }
        if (ctx.variableInitializer().literal().literalFloat() is { } floatContext) {
            return new RapVariableStatement {
                VariableName = ctx.variableDeclaratorId().identifier().GetText(),
                VariableValue = RapFloat.FromParseContext(floatContext)
            };
        }

        if (ctx.variableInitializer().literal().literalInteger() is { } integerContext) {
            return new RapVariableStatement {
                VariableName = ctx.variableDeclaratorId().identifier().GetText(),
                VariableValue = RapInt.FromParseContext(integerContext)
            };
        }
        
        if (ctx.variableInitializer().literal().literalString() is { } stringContext) {
            return new RapVariableStatement {
                VariableName = ctx.variableDeclaratorId().identifier().GetText(),
                VariableValue = RapString.FromParseContext(stringContext)
            };
        }

        throw new Exception($"No value was found for, {ctx.variableDeclaratorId().identifier().GetText()}");
    }
    public static RapVariableStatement FromBinaryFormat(RapBinaryReader reader, bool isArray) {
        if (isArray) {
            return new RapVariableStatement() {
                VariableType = RapValueType.Array,
                VariableName = reader.ReadAsciiZ(),
                VariableValue = reader.ReadRapArray()
            };
        }

        var output = new RapVariableStatement() {
            VariableType = (RapValueType) reader.ReadByte(),
            VariableName = reader.ReadAsciiZ(),
            VariableValue = reader.ReadRapArray()
        };
        
        return output;
    }
    
    

    public RapVariableStatement FromRapContext(PoseidonParser.VariableAssignmentContext ctx) => FromRapFormat(ctx);

    public RapVariableStatement FromBinaryContext(RapBinaryReader reader, bool isArray) {
        if (isArray) {
            VariableType = RapValueType.Array;
            VariableName = reader.ReadAsciiZ();
            VariableValue = reader.ReadRapArray();
            return this;
        }

        VariableType = (RapValueType) reader.ReadByte();
        VariableName = reader.ReadAsciiZ();
        
        IRapSerializable value = VariableType switch {
            RapValueType.String => reader.ReadRapString(),
            RapValueType.Variable => throw new Exception("Variables not supported yet."),
            RapValueType.Float => reader.ReadRapFloat(),
            RapValueType.Long => reader.ReadRapInt(),
            RapValueType.Array => reader.ReadRapArray(),
            _ => throw new Exception("How did we get here?")
        };
        VariableValue = value;
        return this;
    }
}