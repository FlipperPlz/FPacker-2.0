using System.Text;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public class RapDeleteStatement : IRapDeserializable<PoseidonParser.DeleteStatementContext, RapDeleteStatement>, IRapBinarizable<RapDeleteStatement> { 
    public string Label { get; set; }
    
    public RapDeleteStatement FromRapContext(PoseidonParser.DeleteStatementContext ctx) => FromRapFormat(ctx);
    public RapDeleteStatement FromBinaryContext(RapBinaryReader reader, bool defaultFalse = false) {
        Label = reader.ReadAsciiZ();
        return this;
    }

    public static RapDeleteStatement FromBinaryContext(RapBinaryReader reader) => new() {
        Label = reader.ReadAsciiZ()
    };

    public static RapDeleteStatement FromRapFormat(PoseidonParser.DeleteStatementContext ctx) => new() {
        Label = ctx.Start.InputStream.GetText(new Interval(ctx.identifier().Start.StartIndex,
            ctx.identifier().Stop.StopIndex))
    };
    
    public string ToRapFormat() => new StringBuilder("delete ").Append(Label).Append(';').ToString();
}