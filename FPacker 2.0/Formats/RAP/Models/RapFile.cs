using System.Text;
using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models; 

public class RapFile : IRapEntry {
    public List<RapDeleteStatement> GlobalDeleteStatements { get; set; } = new();
    public List<RapVariableStatement> GlobalVariableStatements { get; set; } = new();
    public List<RapClass> GlobalClasses { get; set; } = new();

    private uint enumOffsetPosition;
    private long enumOffsetPositionInStream;
    private int parentEntryCount => GlobalClasses.Count + GlobalDeleteStatements.Count + GlobalVariableStatements.Count;


    private List<long> offsetPositions = new List<long>();
    private List<uint> offsets = new List<uint>();

    
    public string ToRapFormat() {
        var builder = new StringBuilder();
        GlobalDeleteStatements.ForEach(gds => builder.Append(gds.ToRapFormat()).Append('\n'));
        GlobalVariableStatements.ForEach(gvs => builder.Append(gvs.ToRapFormat()).Append('\n'));
        GlobalClasses.ForEach(cc => builder.Append(cc.ToRapFormat()).Append('\n'));
        return builder.ToString();
    }

    public void ToBinaryContext(RapBinaryWriter writer, bool b = false) {
        writer.Write(new byte[] {0x00, (byte) 'r', (byte) 'a', (byte) 'P'});
        writer.Write((uint) 0);
        writer.Write((uint) 8);
        this.enumOffsetPositionInStream = writer.Position;
        writer.Write((uint) 999999);//TODO: SHOULD BE ENUM OFFSET
        
        WriteParentClasses(writer);
        WriteChildClasses(writer);
        this.enumOffsetPosition = (uint) writer.Position;
        offsets.Add(enumOffsetPosition);
        offsetPositions.Add(enumOffsetPositionInStream);

        foreach (var (offset, pos) in offsets.Zip(offsetPositions, static (k, v) => new { k, v }).ToDictionary(static x => x.k, static x => x.v)) {
            writer.Position = pos;
            var osBytes = BitConverter.GetBytes(offset);
            writer.Write(osBytes, 0, osBytes.Length);
        }

        

    }

    private void WriteChildClasses(RapBinaryWriter writer) {
        foreach ( var rapClass in GlobalClasses.Where(static g => !g.ExternalClass)) {
            WriteChildClassesData(writer, rapClass);
        }
    }

    private void WriteChildClassesData(RapBinaryWriter writer, RapClass child) {
        offsets.Add((uint) writer.Position);
        writer.Write((uint) 999999);//TODO: SHOULD BE CLASS OFFSET
        writer.WriteAsciiZ(child.ParentClass ?? string.Empty);
        writer.WriteCompressedInt(child.EntryCount);

        foreach (var array in child.VariableStatements.Where(static g => g.VariableType is RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapArray);
            array.ToBinaryContext(writer, true);
        }
        foreach (var variable in child.VariableStatements.Where(static g => g.VariableType is not RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapValue);
            variable.ToBinaryContext(writer);
        }
        foreach (var externalRapClass in child.ChildClasses.Where(static g => g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapExternClass);
            externalRapClass.ToBinaryContext(writer, true);
        }
        foreach (var rapClass in child.ChildClasses.Where(static g => !g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapClass);
            rapClass.ToBinaryContext(writer);
            offsetPositions.Add(rapClass.OffsetPosition);
        }
        foreach (var rapDeleteStatement in GlobalDeleteStatements) {
            writer.Write((byte) RapEntryType.RapDeleteClass);
            rapDeleteStatement.ToBinaryContext(writer);
        }

        foreach (var childClass in child.ChildClasses.Where(static g => !g.ExternalClass)) {
            WriteChildClassesData(writer, childClass);
        }
    }

    private void WriteParentClasses(RapBinaryWriter writer) {
        writer.WriteAsciiZ(string.Empty);
        writer.WriteCompressedInt(parentEntryCount);
        foreach (var array in GlobalVariableStatements.Where(static g => g.VariableType is RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapArray);
            array.ToBinaryContext(writer, true);
        }
        foreach (var variable in GlobalVariableStatements.Where(static g => g.VariableType is not RapValueType.Array)) {
            writer.Write((byte) RapEntryType.RapValue);
            variable.ToBinaryContext(writer);
        }
        foreach (var externalRapClass in GlobalClasses.Where(static g => g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapExternClass);
            externalRapClass.ToBinaryContext(writer, true);
        }
        foreach (var rapClass in GlobalClasses.Where(static g => !g.ExternalClass)) {
            writer.Write((byte) RapEntryType.RapClass);
            rapClass.ToBinaryContext(writer);
            offsetPositions.Add(rapClass.OffsetPosition);
        }
        foreach (var rapDeleteStatement in GlobalDeleteStatements) {
            writer.Write((byte) RapEntryType.RapDeleteClass);
            rapDeleteStatement.ToBinaryContext(writer);
        }
    }
    
    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) where Tself : IRapDeserializable {
        throw new NotImplementedException();
    }

    public void BinarizeToFile(string filePath) {
        using var fs = File.OpenWrite(filePath);
        using var writer = new RapBinaryWriter(fs);
        ToBinaryContext(writer);
    }
    
    public Tself FromRapContext<Tself>(ParserRuleContext context) where Tself : IRapDeserializable {
        if (context is not PoseidonParser.ComputationalUnitContext ctx) throw new Exception();

        foreach (var statement in ctx.statement()) {
            if (statement.variableAssignment() is { } variableAssignmentContext) 
                GlobalVariableStatements.Add(new RapVariableStatement().FromRapContext<RapVariableStatement>(variableAssignmentContext));
            if (statement.deleteStatement() is { } deleteStatementContext) 
                GlobalDeleteStatements.Add(new RapDeleteStatement().FromRapContext<RapDeleteStatement>(deleteStatementContext));
        }

        GlobalClasses.AddRange(ctx.classDefinition()
            .Select(static classDefinitionContext => new RapClass().FromRapContext<RapClass>(classDefinitionContext)));
        return (Tself) (IRapEntry) this;
    }
}