// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Enforce;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.Enforce.Parse;
using FPacker.Formats.RVMAT.Parse;

namespace FPacker.Formats.Enforce.Models; 

public class EnforceFile {
    public string PBOPath { get; init; }
    public string PBOReferencePath { get; init; }
    public string ObfuscatedPBOPath { get; set; }
    public string ObfuscatedPBORefPath { get; set; }
    public List<int> Modules { get; set; }

    public string SystemPath { get; init; }
    
    public EnforceScript EnforceData { get; private set; }
    
    public EnforceFile(string pboPath, string pboRefPath, string systemPath) {
        PBOPath = pboPath;
        PBOReferencePath = pboRefPath;
        SystemPath = systemPath;
        ParseEnforce();
    }

    private void ParseEnforce() {
        var lexer = new EnforceLexer(CharStreams.fromPath(SystemPath));
        var parser = new EnforceParser(new CommonTokenStream(lexer));
        var listener = new EnforcePreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        EnforceData = listener.Script;
    }
    
    public void ObfuscateScripts() {
        var lexer = new EnforceLexer(CharStreams.fromString(EnforceData.ToEnforceFormat()));
        var tokens = new CommonTokenStream(lexer);
        var rewriter = new TokenStreamRewriter(tokens);
        var parser = new EnforceParser(tokens);

        var listener = new EnforceObfuscationListener() {Rewriter = rewriter};
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());

        ResetData(rewriter.GetText());
    }

    private void ResetData(string getText) {
        var lexer = new EnforceLexer(CharStreams.fromString(getText));
        var parser = new EnforceParser(new CommonTokenStream(lexer));
        var listener = new EnforcePreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        EnforceData = listener.Script;
    }
}

