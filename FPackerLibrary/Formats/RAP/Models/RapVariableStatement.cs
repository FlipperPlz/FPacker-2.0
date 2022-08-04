using System.Text;
using Antlr4.Runtime;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.IO;
using FPackerLibrary.Formats.RAP.Models.Enums;
using FPackerLibrary.Formats.RAP.Models.Values;

namespace FPackerLibrary.Formats.RAP.Models;

public class RapVariableStatement : IRapEntry {
    public string VariableName { get; set; }

    public RapValueType VariableType {
        get {
            return VariableValue switch {
                RapArray => RapValueType.Array,
                RapString => RapValueType.String,
                RapFloat => RapValueType.Float,
                RapInt => RapValueType.Long,
                _ => _parsedValue
            };
        }
        private set => _parsedValue = value;
    }

    private RapValueType _parsedValue;

    public IRapSerializable VariableValue { get; set; }

    public string ToRapFormat() {
        var builder = new StringBuilder(VariableName);
        if (VariableValue is RapArray) builder.Append("[]");
        return builder.Append(" = ").Append(VariableValue.ToRapFormat()).Append(';').ToString();
    }

    public void ToBinaryContext(RapBinaryWriter writer, bool isArray = false) {
        if (VariableValue is RapArray || VariableType is RapValueType.Array || isArray) {
            writer.WriteAsciiZ(VariableName);
            writer.WriteBinarizedValue(VariableValue as RapArray ?? throw new Exception());
            return;
        }
         
        writer.Write(((byte) VariableType));
        writer.WriteAsciiZ(VariableName);
        switch(VariableType) {
            case RapValueType.String:
                writer.WriteBinarizedValue((RapString) VariableValue);
                return;
            case RapValueType.Float:
                writer.WriteBinarizedValue((RapFloat) VariableValue);
                return;
            case RapValueType.Long:
                writer.WriteBinarizedValue((RapInt) VariableValue);
                return;
            case RapValueType.Array:
                writer.WriteBinarizedValue((RapArray) VariableValue);
                return;
            case RapValueType.Variable:
                throw new Exception("Variables not supported yet.");
            default:
                throw new ArgumentOutOfRangeException();
        };
    }

    public Tself FromRapContext<Tself>(ParserRuleContext context) where Tself : IRapDeserializable {
        if (context is not PoseidonParser.VariableAssignmentContext ctx) throw new Exception();
        if (ctx.variableInitializer().arrayInitializer() is { } arrayInitializer) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            
            _parsedValue = RapValueType.Array;
            VariableValue = new RapArray().FromRapContext<RapArray>(arrayInitializer);
            return (Tself) (IRapEntry) this;
        }
        if (ctx.variableInitializer().literal().literalFloat() is { } floatContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            _parsedValue = RapValueType.Float;
            VariableValue = new RapFloat().FromRapContext<RapFloat>(floatContext);
            return (Tself) (IRapEntry) this;
        }
        if (ctx.variableInitializer().literal().literalInteger() is { } integerContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            _parsedValue = RapValueType.Long;
            VariableValue = new RapInt().FromRapContext<RapInt>(integerContext);
            return (Tself) (IRapEntry) this;
        }
        
        if (ctx.variableInitializer().literal().literalString() is { } stringContext) {
            VariableName = ctx.variableDeclaratorId().identifier().GetText();
            _parsedValue = RapValueType.String;
            VariableValue = new RapString().FromRapContext<RapString>(stringContext);
            return (Tself) (IRapEntry) this;
        }

        throw new Exception($"No value was found for, {ctx.variableDeclaratorId().identifier().GetText()}");
    }

    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool isArray = false) where Tself : IRapDeserializable {
        if (isArray) {
            _parsedValue = RapValueType.Array;
            VariableName = reader.ReadAsciiZ();
            VariableValue = reader.ReadBinarizedValue<RapArray>();
            return (Tself) (IRapEntry) this;
        }

        _parsedValue = (RapValueType) reader.ReadByte();
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