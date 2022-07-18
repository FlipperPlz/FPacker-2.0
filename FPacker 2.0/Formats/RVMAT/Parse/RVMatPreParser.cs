using Antlr4.Runtime.Misc;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RVMAT.Parse; 

public class RVMatPreParser : PoseidonBaseListener {
    public RapFile RvmatFile;
    public List<string> TexturePaths = new();

    public override void ExitComputationalUnit(PoseidonParser.ComputationalUnitContext context) =>
        RvmatFile = RapFile.FromRapFormat(context);

    public override void EnterVariableAssignment(PoseidonParser.VariableAssignmentContext context) {
        var varName = context.variableDeclaratorId().identifier().GetText().ToLower();
        switch (varName) {
            case "texture":
                if (context.variableInitializer().literal().literalString() is { } textPathStringCtx) {
                    var texturePath = RapString.FromParseContext(textPathStringCtx).Value;
                    if (!texturePath.StartsWith('#') && !TexturePaths.Contains(texturePath)) TexturePaths.Add(texturePath);
                }
                break;
        }
    }
}