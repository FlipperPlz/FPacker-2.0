﻿using System.Text;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models; 

public class RapFile : RapSerializable, RapDeserializable<PoseidonParser.ComputationalUnitContext, RapFile> {
    public List<RapDeleteStatement> GlobalDeleteStatements { get; set; }
    public List<RapVariableStatement> GlobalVariableStatements { get; set; }
    public List<RapClass> GlobalClasses { get; set; }
    
    public string ToRapFormat() {
        var builder = new StringBuilder();
        GlobalDeleteStatements.ForEach(gds => builder.Append(gds.ToRapFormat()).Append('\n'));
        GlobalVariableStatements.ForEach(gvs => builder.Append(gvs.ToRapFormat()).Append('\n'));
        GlobalClasses.ForEach(cc => builder.Append(cc.ToRapFormat()).Append('\n'));
        return builder.ToString();
    }

    public static RapFile FromRapFormat(PoseidonParser.ComputationalUnitContext ctx) {
        var deleteStatements = new List<RapDeleteStatement>();
        var variableStatements = new List<RapVariableStatement>();

        foreach (var statement in ctx.statement()) {
            if (statement.variableAssignment() is { } variableAssignmentContext) 
                variableStatements.Add(RapVariableStatement.FromRapFormat(variableAssignmentContext));
            if (statement.deleteStatement() is { } deleteStatementContext) 
                deleteStatements.Add(RapDeleteStatement.FromRapFormat(deleteStatementContext));
        }

        var classes = ctx.classDefinition().Select(static classDefinitionContext => RapClass.FromRapFormat(classDefinitionContext)).ToList();

        return new RapFile() {
            GlobalDeleteStatements = deleteStatements,
            GlobalVariableStatements = variableStatements,
            GlobalClasses = classes
        };
    }

    public RapFile FromRapContext(PoseidonParser.ComputationalUnitContext ctx) => FromRapFormat(ctx);
}