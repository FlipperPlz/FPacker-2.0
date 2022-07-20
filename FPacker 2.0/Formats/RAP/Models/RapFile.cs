using System.Text;
using Antlr4.Runtime;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public class RapFile : IRapEntry {
    public List<RapDeleteStatement> GlobalDeleteStatements { get; set; } = new();
    public List<RapVariableStatement> GlobalVariableStatements { get; set; } = new();
    public List<RapClass> GlobalClasses { get; set; } = new();
    
    public string ToRapFormat() {
        var builder = new StringBuilder();
        GlobalDeleteStatements.ForEach(gds => builder.Append(gds.ToRapFormat()).Append('\n'));
        GlobalVariableStatements.ForEach(gvs => builder.Append(gvs.ToRapFormat()).Append('\n'));
        GlobalClasses.ForEach(cc => builder.Append(cc.ToRapFormat()).Append('\n'));
        return builder.ToString();
    }

    public void ToBinaryContext(RapBinaryWriter writer) {
        throw new NotImplementedException();
    }
    
    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) where Tself : IRapDeserializable {
        throw new NotImplementedException();
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