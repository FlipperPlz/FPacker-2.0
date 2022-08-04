// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using System.Security.Cryptography;
using Antlr4.Runtime;
using FPackerLibrary.Antlr.Enforce;

namespace FPackerLibrary.Formats.Enforce.Parse; 

public class EnforceObfuscationListener : EnforceParserBaseListener {
    internal TokenStreamRewriter Rewriter { get; init; }
    private Dictionary<string, string>? _localMappings = null;


    public override void EnterLocalVariableDeclaration(EnforceParser.LocalVariableDeclarationContext context) {
        if (context.localVariableDeclarationAssumptuative() is { } localVariableDeclarationAssCtx) {
            if (localVariableDeclarationAssCtx.identifier() is not null) {
                _localMappings.Add(localVariableDeclarationAssCtx.GetText(), RandomString());
            }
        } else if(context.localVariableDeclarationRegular() is { } localVariableDeclarationRegular) {
            foreach (var variableDeclarator in localVariableDeclarationRegular.variableDeclarators().variableDeclarator()) {
                if (variableDeclarator.variableDeclaratorId().identifier() is { } identifier) {
                    _localMappings.Add(identifier.GetText(), RandomString());

                }
            }
        }
    }

    public override void ExitIdentifier(EnforceParser.IdentifierContext context) {
        if(_localMappings is not null && _localMappings.ContainsKey(context.GetText())) 
            Rewriter.Replace(context.Start, context.Stop, _localMappings[context.GetText()]);
    }


    public override void EnterMethodBody(EnforceParser.MethodBodyContext context) {
        _localMappings = new Dictionary<string, string>();
    }

    public override void ExitMethodBody(EnforceParser.MethodBodyContext context) {
        _localMappings = null;
    }
    
    private static string RandomString(int stringLength = 8) {
        // ReSharper disable once StringLiteralTypo
        var allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        var rnd = new byte[stringLength];
        using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(rnd);
        
        var allowable = allowableChars.ToCharArray();
        var l = allowable.Length;
        var chars = new char[stringLength];
        for (var i = 0; i < stringLength; i++)
            chars[i] = allowable[rnd[i] % l];

        var generatedString = new string(chars);
        
        return generatedString;
    }
}