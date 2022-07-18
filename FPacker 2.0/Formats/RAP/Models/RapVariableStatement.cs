using System.Text;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models;

public struct RapVariableStatement : RapSerializable,
    RapDeserializable<PoseidonParser.VariableAssignmentContext, RapVariableStatement> {
    public string VariableName { get; set; }
    public RapSerializable VariableValue { get; set; }

    public readonly string ToRapFormat() {
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

    public RapVariableStatement FromRapContext(PoseidonParser.VariableAssignmentContext ctx) => FromRapFormat(ctx);
}