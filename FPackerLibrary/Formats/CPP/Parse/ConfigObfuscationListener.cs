using Antlr4.Runtime;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.Enforce.Models;
using FPackerLibrary.Formats.RAP.Models.Values;
using FPackerLibrary.Models.AddonFiles;

namespace FPackerLibrary.Formats.CPP.Parse;

public class ConfigObfuscationListener : PoseidonBaseListener {
    internal TokenStreamRewriter Rewriter { get; init; }
    internal Dictionary<string, string> ObfuscatedPaths { get; init; } = new();
    internal List<EnforceFileSerializable> EnforceFiles { get; init; } = new();

    public override void ExitLiteralString(PoseidonParser.LiteralStringContext context) {
        var str = new RapString().FromRapContext<RapString>(context);
        if (ObfuscatedPaths.ContainsKey(str.Value)) {
            Rewriter.Replace(context.Start, context.Stop, $"\"{ObfuscatedPaths[str.Value]}\"");

        }

        base.ExitLiteralString(context);
    }

    public override void ExitVariableAssignment(PoseidonParser.VariableAssignmentContext context) {
        if(context.variableDeclaratorId() is null) return;
        if(context.variableDeclaratorId().identifier() is null) return;

        var varName = context.variableDeclaratorId().identifier().GetText().ToLower();
        switch(varName) {
            case "files":
                if (context.variableInitializer().arrayInitializer() is { } scriptArrayCtx) {
                    if (context.Parent.Parent.Parent is PoseidonParser.ClassDefinitionContext clazz) {
                        switch (clazz.identifier().GetText().ToLower()) {
                            case "engine" + "script" + "module":
                                var arr = new RapArray();
                                foreach (var enforceFile in EnforceFiles) {
                                    if (!enforceFile.Modules.Contains(1)) continue;
                                    arr.Value.Add(new RapString(enforceFile.ObfuscatedPBORefPath));

                                }
                                Rewriter.Replace(scriptArrayCtx.Start, scriptArrayCtx.Stop, arr.ToRapFormat());
                                break;
                            case "game" + "lib" + "script" + "module":
                                var arr1 = new RapArray();
                                foreach (var enforceFile in EnforceFiles) {
                                    if (!enforceFile.Modules.Contains(2)) continue;
                                    arr1.Value.Add(new RapString(enforceFile.ObfuscatedPBORefPath));

                                }
                                Rewriter.Replace(scriptArrayCtx.Start, scriptArrayCtx.Stop, arr1.ToRapFormat());
                                break;
                            case "game" + "script" + "module":
                                var arr3 = new RapArray();
                                foreach (var enforceFile in EnforceFiles) {
                                    if (!enforceFile.Modules.Contains(3)) continue;
                                    arr3.Value.Add(new RapString(enforceFile.ObfuscatedPBORefPath));

                                }
                                Rewriter.Replace(scriptArrayCtx.Start, scriptArrayCtx.Stop, arr3.ToRapFormat());
                                break;
                            case "world" + "script" + "module":
                                var arr4 = new RapArray();
                                foreach (var enforceFile in EnforceFiles) {
                                    if (!enforceFile.Modules.Contains(4)) continue;
                                    arr4.Value.Add(new RapString(enforceFile.ObfuscatedPBORefPath));
                                }
                                Rewriter.Replace(scriptArrayCtx.Start, scriptArrayCtx.Stop, arr4.ToRapFormat());
                                break;
                            case "mission" + "script" + "module":
                                var arr5 = new RapArray();
                                foreach (var enforceFile in EnforceFiles) {
                                    if (!enforceFile.Modules.Contains(5)) continue;
                                    arr5.Value.Add(new RapString() {
                                        Value = enforceFile.ObfuscatedPBORefPath
                                    });
                                }
                                Rewriter.Replace(scriptArrayCtx.Start, scriptArrayCtx.Stop, arr5.ToRapFormat());
                                break;
                        }
                    } else throw new NotImplementedException();
                }
                break;

                
        }
    }
}