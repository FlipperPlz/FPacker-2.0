using Antlr4.Runtime.Misc;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.Formats.RAP.Models.Values;

namespace FPackerLibrary.Formats.CPP.Parse; 

public class ConfigPreParser : PoseidonBaseListener {
    public RapFile ConfigFile;
    public readonly List<string> EstablishedPrefixes = new();
    
    public readonly List<string> TexturePaths = new();
    public readonly List<string> ModelPaths = new();
    public readonly List<string> MaterialPaths = new();
    public readonly List<string> SoundSamplePaths = new();
    
    public readonly List<string> EngineScriptPaths = new();
    public readonly List<string> GameLibScriptPaths = new();
    public readonly List<string> GameScriptPaths = new();
    public readonly List<string> WorldScriptPaths = new();
    public readonly List<string> MissionScriptPaths = new();
    
    
    public override void ExitComputationalUnit(PoseidonParser.ComputationalUnitContext context) => ConfigFile = new RapFile().FromRapContext<RapFile>(context);
    
    public override void EnterVariableAssignment(PoseidonParser.VariableAssignmentContext context) {
        var varName = context.variableDeclaratorId().identifier().GetText().ToLower();
        switch (varName) {
            case "model":
                if (context.variableInitializer().literal().literalString() is { } mdlPathStringCtx) {
                    var modelPath = new RapString().FromRapContext<RapString>(mdlPathStringCtx);
                    if(!ModelPaths.Contains(modelPath.Value)) ModelPaths.Add(modelPath.Value);
                }
                break;
            case "hidden" + "selections" + "textures":
                if (context.variableInitializer().arrayInitializer() is { } hiddenSelectionsTexturesArrayCtx) {
                    new RapArray().FromRapContext<RapArray>(hiddenSelectionsTexturesArrayCtx).FlattenToStrings(true)
                        .ForEach(p => { if (!p.Value.StartsWith('#') && !TexturePaths.Contains(p.Value)) TexturePaths.Add(p.Value); });
                }
                break;
            case "hidden" + "selections" + "materials":
                if (context.variableInitializer().arrayInitializer() is { } hiddenSelectionsMaterialsArrayCtx) {
                    new RapArray().FromRapContext<RapArray>(hiddenSelectionsMaterialsArrayCtx).FlattenToStrings(true)
                        .ForEach(p => { if (!p.Value.StartsWith('#') && !MaterialPaths.Contains(p.Value)) MaterialPaths.Add(p.Value); });
                }
                break;
            case "health" + "levels":
                if (context.variableInitializer().arrayInitializer() is { } healthLevelsArrayCtx) {
                    new RapArray().FromRapContext<RapArray>(healthLevelsArrayCtx).FlattenToStrings(true)
                        .ForEach(p => { if (!p.Value.StartsWith('#') && !MaterialPaths.Contains(p.Value)) MaterialPaths.Add(p.Value); });
                }
                break;
            case "samples":
                if (context.variableInitializer().arrayInitializer() is { } soundSamplesArrayCtx) {
                    new RapArray().FromRapContext<RapArray>(soundSamplesArrayCtx).FlattenToStrings(true)
                        .ForEach(p => { if (!p.Value.StartsWith('#') && !SoundSamplePaths.Contains(p.Value)) SoundSamplePaths.Add(p.Value); });
                }
                break;
            case "files":
                if (context.variableInitializer().arrayInitializer() is { } scriptArrayCtx) {
                    if (context.Parent.Parent.Parent is PoseidonParser.ClassDefinitionContext clazz) {
                        switch (clazz.identifier().GetText().ToLower()) {
                            case "engine" + "script" + "module":
                                new RapArray().FromRapContext<RapArray>(scriptArrayCtx).FlattenToStrings(true)
                                    .ForEach(p => { if (!p.Value.StartsWith('#') && !EngineScriptPaths.Contains(p.Value)) EngineScriptPaths.Add(p.Value); });
                                break;
                            case "game" + "lib" + "script" + "module":
                                new RapArray().FromRapContext<RapArray>(scriptArrayCtx).FlattenToStrings(true)
                                    .ForEach(p => { if (!p.Value.StartsWith('#') && !GameLibScriptPaths.Contains(p.Value)) GameLibScriptPaths.Add(p.Value); });
                                break;
                            case "game" + "script" + "module":
                                new RapArray().FromRapContext<RapArray>(scriptArrayCtx).FlattenToStrings(true)
                                    .ForEach(p => { if (!p.Value.StartsWith('#') && !GameScriptPaths.Contains(p.Value)) GameScriptPaths.Add(p.Value); });
                                break;
                            case "world" + "script" + "module":
                                new RapArray().FromRapContext<RapArray>(scriptArrayCtx).FlattenToStrings(true)
                                    .ForEach(p => { if (!p.Value.StartsWith('#') && !WorldScriptPaths.Contains(p.Value)) WorldScriptPaths.Add(p.Value); });
                                break;
                            case "mission" + "script" + "module":
                                new RapArray().FromRapContext<RapArray>(scriptArrayCtx).FlattenToStrings(true)
                                    .ForEach(p => { if (!p.Value.StartsWith('#') && !MissionScriptPaths.Contains(p.Value)) MissionScriptPaths.Add(p.Value); });
                                break;
                        }
                    } else throw new NotImplementedException();
                }
                break;

                
        }
    }

    public override void EnterClassDefinition(PoseidonParser.ClassDefinitionContext context) {
        if (context.Parent.Parent is PoseidonParser.ClassDefinitionContext parentClassCtx && parentClassCtx.identifier().GetText().ToLower() == "cfg" + "patches") { 
            EstablishedPrefixes.Add(context.identifier().GetText());
            return;
        }
    }

}