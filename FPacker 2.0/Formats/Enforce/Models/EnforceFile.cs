// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.Enforce.Parse;
using FPacker.Formats.RVMAT.Parse;

namespace FPacker.Formats.Enforce.Models; 

public class EnforceFile {
    public string PBOPath { get; init; }
    public string PBOReferencePath { get; init; }
    public string ObfuscatedPBOPath { get; private set; }
    public string ObfuscatedPBORefPath { get; private set; }

    public string SystemPath { get; init; }
    
    public EnforceScript EnforceData { get; private set; }
    
    public EnforceFile(string pboPath, string pboRefPath, string systemPath) {
        PBOPath = pboPath;
        PBOReferencePath = pboRefPath;
        SystemPath = systemPath;
        ParseEnforce();
    }

    private void ParseEnforce() {
        var lexer = new PoseidonLexer(CharStreams.fromPath(SystemPath));
        var parser = new PoseidonParser(new CommonTokenStream(lexer));
        var listener = new EnforcePreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        EnforceData = listener.Script;
    }
}