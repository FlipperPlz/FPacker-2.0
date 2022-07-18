using System.Text;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;

namespace FPacker.Formats.RAP.Models; 

public struct RapDeleteStatement : RapSerializable, RapDeserializable<PoseidonParser.DeleteStatementContext, RapDeleteStatement> {
    public string Label { get; set; }
    
    public RapDeleteStatement FromRapContext(PoseidonParser.DeleteStatementContext ctx) => FromRapFormat(ctx);
    
    public static RapDeleteStatement FromRapFormat(PoseidonParser.DeleteStatementContext ctx) => new() {
        Label = ctx.Start.InputStream.GetText(new Interval(ctx.identifier().Start.StartIndex,
            ctx.identifier().Stop.StopIndex))
    };
    
    public readonly string ToRapFormat() => new StringBuilder("delete ").Append(Label).Append(';').ToString();

}