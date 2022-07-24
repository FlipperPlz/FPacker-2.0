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
using FPacker.Formats.Enforce.Models;
using FPacker.Formats.Enforce.Parse;

namespace FPacker.Models.AddonFiles; 

public class EnforceFile : BaseAddonFileSerializable<EnforceScript> {
    public List<int> Modules { get; set; }
    
    private void ResetData(string getText) {
        var lexer = new EnforceLexer(CharStreams.fromString(getText));
        var parser = new EnforceParser(new CommonTokenStream(lexer));
        var listener = new EnforcePreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.Script;
    }

    public EnforceFile(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath, systemPath) { }
    
    protected override void InitializeObject(Stream stream) {
        var lexer = new EnforceLexer(CharStreams.fromPath(SystemPath));
        var parser = new EnforceParser(new CommonTokenStream(lexer));
        var listener = new EnforcePreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        ObjectBase = listener.Script;
    }

    public override void ObfuscateObject() {
        var lexer = new EnforceLexer(CharStreams.fromString(ObjectBase.ToEnforceFormat()));
        var tokens = new CommonTokenStream(lexer);
        var rewriter = new TokenStreamRewriter(tokens);
        var parser = new EnforceParser(tokens);

        var listener = new EnforceObfuscationListener() {Rewriter = rewriter};
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());

        ResetData(rewriter.GetText());
    }
}

