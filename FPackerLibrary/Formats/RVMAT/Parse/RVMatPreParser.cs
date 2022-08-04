using Antlr4.Runtime.Misc;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.Formats.RAP.Models.Values;

namespace FPackerLibrary.Formats.RVMAT.Parse; 

public class RVMatPreParser : PoseidonBaseListener {
    public RapFile RvmatFile;
    public List<string> TexturePaths = new();

    public override void ExitComputationalUnit(PoseidonParser.ComputationalUnitContext context) =>
        RvmatFile = new RapFile().FromRapContext<RapFile>(context);

    public override void EnterVariableAssignment(PoseidonParser.VariableAssignmentContext context) {
        var varName = context.variableDeclaratorId().identifier().GetText().ToLower();
        switch (varName) {
            case "texture":
                if (context.variableInitializer().literal().literalString() is { } textPathStringCtx) {
                    var texturePath = new RapString().FromRapContext<RapString>(textPathStringCtx).Value;
                    if (!texturePath.StartsWith('#') && !TexturePaths.Contains(texturePath)) TexturePaths.Add(texturePath);
                }
                break;
        }
    }
}