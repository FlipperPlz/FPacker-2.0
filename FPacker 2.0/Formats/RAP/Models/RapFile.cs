using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.CPP.Parse;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models; 

public class RapFile : IRapEntry {
    public List<RapDeleteStatement> GlobalDeleteStatements { get; set; } = new();
    public List<RapVariableStatement> GlobalVariableStatements { get; set; } = new();
    public List<RapClass> GlobalClasses { get; set; } = new();
    public int FileEntryCount => GlobalClasses.Count + GlobalDeleteStatements.Count + GlobalVariableStatements.Count;
    
    public List<RapClass> GlobalExternalClasses => GlobalClasses.Where(static c => c.ExternalClass).ToList();
    public List<RapClass> GlobalRegularClasses => GlobalClasses.Where(static c => !c.ExternalClass).ToList();
    public List<RapVariableStatement> GlobalArrayStatements => GlobalVariableStatements.Where(static v => v.VariableType is RapValueType.Array).ToList();
    public List<RapVariableStatement> GlobalTokenStatements => GlobalVariableStatements.Where(static v => v.VariableType is not RapValueType.Array).ToList();

    
    public string ToRapFormat() {
        var builder = new StringBuilder();
        GlobalDeleteStatements.ForEach(gds => builder.Append(gds.ToRapFormat()).Append('\n'));
        GlobalVariableStatements.ForEach(gvs => builder.Append(gvs.ToRapFormat()).Append('\n'));
        GlobalClasses.ForEach(cc => builder.Append(cc.ToRapFormat()).Append('\n'));
        return builder.ToString();
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
    
    #region read raP
    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) where Tself : IRapDeserializable {
        if (!reader.ReadHeader()) throw new Exception("An invalid raP file was passed to RapBinaryParser.");
        if (reader.IsOperationFlashpointFormat()) throw new Exception("Operation Flashpoint is not supported.");
        if(reader.ReadUInt32() != 0 || reader.ReadUInt32() != 8) throw new Exception("Expected bytes 0 and 8.");
        var enumOffset = reader.ReadUInt32();
        if (!ReadParentClasses(reader)) Console.WriteLine("No parent classes were found.");
        if (!ReadChildClasses(reader)) Console.WriteLine("No child classes were found.");
        return (Tself) (IRapEntry) this;
    }
    
    private bool ReadParentClasses(RapBinaryReader reader) {
        Console.WriteLine(reader.ReadAsciiZ());
        var parentEntryCount = reader.ReadCompressedInteger();

        for (var i = 0; i < parentEntryCount; ++i) {
            switch ((RapEntryType) reader.ReadByte()) {
                case RapEntryType.RapClass:
                    GlobalClasses.Add(reader.ReadBinarizedEntry<RapClass>());
                    break;
                case RapEntryType.RapExternClass:
                    GlobalClasses.Add(reader.ReadBinarizedEntry<RapClass>(true));
                    break;
                case RapEntryType.RapValue:
                    GlobalVariableStatements.Add(reader.ReadBinarizedEntry<RapVariableStatement>());
                    break;
                case RapEntryType.RapDeleteClass:
                    GlobalDeleteStatements.Add(reader.ReadBinarizedEntry<RapDeleteStatement>());
                    break;
                case RapEntryType.RapArray:
                    GlobalVariableStatements.Add(reader.ReadBinarizedEntry<RapVariableStatement>(true));
                    break;
                default: throw new NotImplementedException();
            }
            
        }
        return parentEntryCount > 0;
    }
    
    private bool ReadChildClasses(RapBinaryReader reader) {
        GlobalClasses.ForEach(c => LoadChildClasses(reader, c));
        return GlobalClasses.Count > 0;
    }

    private void LoadChildClasses(RapBinaryReader reader, RapClass obj) {
        if(obj.ExternalClass) return;
        
        reader.Position = obj.BinaryOffset;

        var parent = reader.ReadAsciiZ();
        obj.ParentClass = (parent == string.Empty) ? null : parent;
        var EntryCount = reader.ReadCompressedInteger();

        for (var i = 0; i < EntryCount; ++i) AddEntryToClass(reader, obj);

        obj.ChildClasses.ForEach(c => LoadChildClasses(reader, c));
    }

    private void AddEntryToClass(RapBinaryReader reader, RapClass child) {
        if(child.ExternalClass) return;

        var entryType = (RapEntryType)reader.ReadByte();

        switch (entryType) {
            case RapEntryType.RapArray:
                child.VariableStatements.Add(reader.ReadBinarizedEntry<RapVariableStatement>(true));
                break;
            case RapEntryType.RapValue:
                child.VariableStatements.Add(reader.ReadBinarizedEntry<RapVariableStatement>());
                break;
            case RapEntryType.RapDeleteClass:
                child.DeleteStatements.Add(reader.ReadBinarizedEntry<RapDeleteStatement>());
                break;
            case RapEntryType.RapExternClass:
                child.ChildClasses.Add(reader.ReadBinarizedEntry<RapClass>(true));
                break;
            case RapEntryType.RapClass:
                child.ChildClasses.Add(reader.ReadBinarizedEntry<RapClass>());
                break;
            default:
                throw new Exception(entryType.ToString());
        }
    }
    
    #endregion
    #region write raP

    public void ToBinaryContext(RapBinaryWriter writer, bool b = false) {
        writer.Write(new byte[] {0x00, (byte) 'r', (byte) 'a', (byte) 'P'});
        writer.Write((uint) 0);
        writer.Write((uint) 8);
        var enumOffsetPosition = writer.Position;
        writer.Write((uint) 999999); //Write Enum offset. will be changed later
        
        WriteParentClasses(writer);
        WriteChildClasses(writer);
        
        var enumOffset = (uint) writer.Position;
        writer.Position = enumOffsetPosition;
        writer.Write(BitConverter.GetBytes(enumOffset), 0, 4);
        writer.Position = enumOffset;
        
        writer.Write((uint) 0);
    }

    private void WriteParentClasses(RapBinaryWriter writer) {
        writer.WriteAsciiZ();
        writer.WriteCompressedInt(FileEntryCount);
        foreach (var externalClass in GlobalExternalClasses) {
            writer.Write((byte) RapEntryType.RapExternClass);
            writer.WriteBinarizedEntry(externalClass, true);
        }

        foreach (var clazz in GlobalRegularClasses) {
            writer.Write((byte) RapEntryType.RapClass);
            writer.WriteBinarizedEntry(clazz);
        }

        foreach (var deleteStatement in GlobalDeleteStatements) {
            writer.Write((byte) RapEntryType.RapDeleteClass);
            writer.WriteBinarizedEntry(deleteStatement);
        }

        foreach (var variableStatement in GlobalTokenStatements) {
            writer.Write((byte) RapEntryType.RapValue);
            writer.WriteBinarizedEntry(variableStatement, false);
        }

        foreach (var arrayStatement in GlobalArrayStatements) {
            writer.Write((byte) RapEntryType.RapArray);
            writer.WriteBinarizedEntry(arrayStatement, true);
        }
    }

    private void WriteChildClasses(RapBinaryWriter writer) {
        foreach (var globalClazz in GlobalRegularClasses) {
            SaveChildClasses(writer, globalClazz);
        }
    }

    private void SaveChildClasses(RapBinaryWriter writer, RapClass globalClazz) {
        globalClazz.BinaryOffset = (uint) writer.Position;
        writer.Position = globalClazz.BinaryOffsetPosition;
        writer.Write(BitConverter.GetBytes(globalClazz.BinaryOffset), 0, 4);
        writer.Position = globalClazz.BinaryOffset;
        
        writer.WriteAsciiZ(globalClazz.ParentClass ?? "");
        writer.WriteCompressedInt(globalClazz.EntryCount);

        foreach (var externalClass in globalClazz.ChildExternalClasses) {
            writer.Write((byte) RapEntryType.RapExternClass);
            writer.WriteBinarizedEntry(externalClass, true);
        }

        foreach (var clazz in globalClazz.ChildRegularClasses) {
            writer.Write((byte) RapEntryType.RapClass);
            writer.WriteBinarizedEntry(clazz);
        }

        foreach (var deleteStatement in globalClazz.DeleteStatements) {
            writer.Write((byte) RapEntryType.RapDeleteClass);
            writer.WriteBinarizedEntry(deleteStatement);
        }

        foreach (var variableStatement in globalClazz.TokenStatements) {
            writer.Write((byte) RapEntryType.RapValue);
            writer.WriteBinarizedEntry(variableStatement, false);
        }

        foreach (var arrayStatement in globalClazz.ArrayStatements) {
            writer.Write((byte) RapEntryType.RapArray);
            writer.WriteBinarizedEntry(arrayStatement, true);
        }
        
        globalClazz.ChildRegularClasses.ForEach(c => SaveChildClasses(writer, c));
    }

    #endregion
    
    public void BinarizeToFile(string filePath) {
        using var fs = File.OpenWrite(filePath);
        using var writer = new RapBinaryWriter(fs);
        ToBinaryContext(writer);
    }

    public static RapFile OpenFile(string filePath) {
        using (var reader = new RapBinaryReader(File.ReadAllBytes(filePath))) {

            if (reader.ReadHeader()) {
                reader.Position -= 4;
                return new RapFile().FromBinaryContext<RapFile>(reader);
            }
            reader.Close();
        };
        
        
        var lexer = new PoseidonLexer(CharStreams.fromPath(filePath));
        var tokens = new CommonTokenStream(lexer);
        var parser = new PoseidonParser(tokens);
        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        return listener.ConfigFile;
    }
}