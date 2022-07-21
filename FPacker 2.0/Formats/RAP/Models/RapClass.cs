using System.Text;
using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;

namespace FPacker.Formats.RAP.Models; 

public class RapClass : IRapEntry {
    public string ClassName { get; set; }
    public string? ParentClass { get; set; } = null;
    public bool ExternalClass { get; set; } = false;
    public List<RapDeleteStatement> DeleteStatements { get; init; } = new();
    public List<RapVariableStatement> VariableStatements { get; init; } = new();
    public List<RapClass> ChildClasses { get; init; } = new();

    public uint BinaryOffset { get; set; }

    public int EntryCount => ChildClasses.Count + VariableStatements.Count + DeleteStatements.Count;


    public long OffsetPosition { get; private set; }
    
    public string ToRapFormat() {
        var builder = new StringBuilder("class ").Append(ClassName);
        if (ParentClass is not null) builder.Append(" : ").Append(ParentClass);
        if (ExternalClass) return builder.Append(';').ToString();
        builder.Append(" {\n");
        DeleteStatements.ForEach(gds => builder.Append(gds.ToRapFormat()).Append('\n'));
        VariableStatements.ForEach(gvs => builder.Append(gvs.ToRapFormat()).Append('\n'));
        ChildClasses.ForEach(cc => builder.Append(cc.ToRapFormat()).Append('\n'));
        return builder.Append('}').Append(';').ToString();
    }
    
    

    public Tself FromRapContext<Tself>(ParserRuleContext context) where Tself : IRapDeserializable {
        if (context is not PoseidonParser.ClassDefinitionContext ctx) throw new Exception();
        ClassName = ctx.identifier().GetText();
        ParentClass = null;

        if (ctx.classExtension() is { } classExtensionCtx) {
            if (classExtensionCtx.identifier() is { } classExtensionIdentifier) {
                ParentClass = classExtensionIdentifier.GetText();
            }
        }

        if (ctx.classBlock() is not { } classBlock) {
            ExternalClass = true;
            return (Tself) (IRapEntry) this;
        }


        foreach (var statement in classBlock.statement()) {
            if (statement.variableAssignment() is { } variableAssignmentContext) 
                VariableStatements.Add(new RapVariableStatement().FromRapContext<RapVariableStatement>(variableAssignmentContext));
            if (statement.deleteStatement() is { } deleteStatementContext) 
                DeleteStatements.Add(new RapDeleteStatement().FromRapContext<RapDeleteStatement>(deleteStatementContext));
        }

        ChildClasses.AddRange(classBlock.classDefinition().Select(static classDefCtx => new RapClass().FromRapContext<RapClass>(classDefCtx)));
        ExternalClass = false;

        return (Tself) (IRapEntry) this;
    }
    
    public void ToBinaryContext(RapBinaryWriter writer, bool externalClass = false) {
        writer.WriteAsciiZ(ClassName);
        if (!ExternalClass || externalClass) {
            OffsetPosition = writer.Position;
            writer.Write((uint) BinaryOffset);
        }
    }

    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool externalClass = false) where Tself : IRapDeserializable {
        ExternalClass = externalClass;
        ClassName = reader.ReadAsciiZ();
        if (!ExternalClass) BinaryOffset = reader.ReadUInt32();
        return (Tself) (IRapEntry) this;
    }

    public void WriteEntries(RapBinaryWriter writer) {
        foreach (var array in VariableStatements.Where(static g => g.VariableType is RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapArray);
            array.ToBinaryContext(writer, true);
        }
        foreach (var variable in VariableStatements.Where(static g => g.VariableType is not RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapValue);
            variable.ToBinaryContext(writer, false);
        }
        foreach (var externalRapClass in ChildClasses.Where(static g => g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapExternClass);
            externalRapClass.ToBinaryContext(writer, true);
        }
        foreach (var rapClass in ChildClasses.Where(static g => !g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapClass);
            rapClass.ToBinaryContext(writer, true);
        }
        foreach (var rapDeleteStatement in DeleteStatements) {
            writer.Write((byte) RapEntryType.RapDeleteClass);
            rapDeleteStatement.ToBinaryContext(writer, true);
        }
    }
}