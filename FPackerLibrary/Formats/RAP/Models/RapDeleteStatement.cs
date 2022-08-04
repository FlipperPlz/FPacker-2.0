using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public class RapDeleteStatement : IRapEntry { 
    public string Label { get; set; }
    
    public string ToRapFormat() => new StringBuilder("delete ").Append(Label).Append(';').ToString();
    
    public void ToBinaryContext(RapBinaryWriter writer, bool defaultFalse = false) => writer.WriteAsciiZ(Label);

    public Tself FromRapContext<Tself>(ParserRuleContext context) where Tself : IRapDeserializable {
        if (context is not PoseidonParser.DeleteStatementContext ctx) throw new Exception();
        Label = ctx.Start.InputStream.GetText(new Interval(ctx.identifier().Start.StartIndex,
            ctx.identifier().Stop.StopIndex));
        return (Tself) (IRapEntry) this;
    }

    public Tself FromBinaryContext<Tself>(RapBinaryReader reader, bool defaultFalse = false) where Tself : IRapDeserializable {
        Label = reader.ReadAsciiZ();
        return (Tself) (IRapEntry) this;
    }
}