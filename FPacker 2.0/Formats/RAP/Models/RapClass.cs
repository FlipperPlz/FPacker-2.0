﻿using System.Text;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public class RapClass : IRapSerializable, IRapDeserializable<PoseidonParser.ClassDefinitionContext, RapClass>, IRapBinarizable<RapClass> {
    public string ClassName { get; set; }
    public string? ParentClass { get; set; } = null;
    public bool ExternalClass { get; init; } = false;
    public List<RapDeleteStatement> DeleteStatements { get; init; } = new();
    public List<RapVariableStatement> VariableStatements { get; init; } = new();
    public List<RapClass> ChildClasses { get; init; } = new();

    public uint BinaryOffset { get; set; } 
    public int EntryCount { get; set; } 
    
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

    public static RapClass FromRapFormat(PoseidonParser.ClassDefinitionContext ctx) {
        var className = ctx.identifier().GetText();
        string? classExtension = null;

        if (ctx.classExtension() is { } classExtensionCtx) {
            if (classExtensionCtx.identifier() is { } classExtensionIdentifier) {
                classExtension = classExtensionIdentifier.GetText();
            }
        }

        if (ctx.classBlock() is not { } classBlock) {
            return new RapClass {
                ClassName = className,
                ParentClass = classExtension,
                ExternalClass = true
            };
        }

        var deleteStatements = new List<RapDeleteStatement>();
        var variableStatements = new List<RapVariableStatement>();

        foreach (var statement in classBlock.statement()) {
            if (statement.variableAssignment() is { } variableAssignmentContext) 
                variableStatements.Add(RapVariableStatement.FromRapFormat(variableAssignmentContext));
            if (statement.deleteStatement() is { } deleteStatementContext) 
                deleteStatements.Add(RapDeleteStatement.FromRapFormat(deleteStatementContext));
        }

        return new RapClass() {
            ClassName = className,
            ParentClass = classExtension,
            ExternalClass = false,
            ChildClasses = classBlock.classDefinition().Select(static classDefCtx => FromRapFormat(classDefCtx)).ToList(),
            DeleteStatements = deleteStatements,
            VariableStatements = variableStatements
        };
    }
    
    private static RapClass FromBinaryFormat(RapBinaryReader reader, bool externalClass) => externalClass 
        ? new RapClass() {
            ExternalClass = true,
            ClassName = reader.ReadAsciiZ()
        } 
        : new RapClass {
            ClassName = reader.ReadAsciiZ(),
            BinaryOffset = reader.ReadUInt32()
        };


    public RapClass FromRapContext(PoseidonParser.ClassDefinitionContext ctx) => FromRapFormat(ctx);
    public RapClass FromBinaryContext(RapBinaryReader reader, bool externalClass) => FromBinaryFormat(reader, externalClass);
}