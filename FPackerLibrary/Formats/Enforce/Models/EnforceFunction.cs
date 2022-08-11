// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using System.Text;
using Antlr4.Runtime.Misc;
using FPackerLibrary.Antlr.Enforce;

namespace FPackerLibrary.Formats.Enforce.Models; 

public class EnforceFunction : IEnforceDeserializable<EnforceParser.MethodDeclarationContext, EnforceFunction> {
    public List<string> FunctionModifiers { get; set; }
    public string FunctionType { get; set; }
    public string FunctionName { get; set; }
    public List<EnforceFunctionParameter> FunctionParameters { get; set; }
    public string FunctionBody { get; set; }
    
    public EnforceFunction FromEnforceContext(EnforceParser.MethodDeclarationContext ctx) => FromContext(ctx);

    public static EnforceFunction FromContext(EnforceParser.MethodDeclarationContext ctx) {
        var funcModifiers = new List<string>();
        var funcType = "";
        var funcName = "";
        var funcParameters = new List<EnforceFunctionParameter>();
        var funcBody = "";
        
        if (ctx.methodModifier() is { } modList) {
            funcModifiers.AddRange(modList.Select(mod => ctx.Start.InputStream.GetText(new Interval(mod.Start.StartIndex, mod.Stop.StopIndex))));
        }

        if (ctx.typeTypeOrVoid() is { } type) {
            funcType = ctx.Start.InputStream.GetText(new Interval(type.Start.StartIndex, type.Stop.StopIndex));
        }

        if (ctx.identifier() is { } name) {
            funcName = ctx.Start.InputStream.GetText(new Interval(name.Start.StartIndex, name.Stop.StopIndex));
        }

        if (ctx.formalParameters() is not null && ctx.formalParameters().formalParameterList() is not null && ctx.formalParameters().formalParameterList() is { } formalParameterList) {
            funcParameters.AddRange(formalParameterList.formalParameter().Select(EnforceFunctionParameter.FromContext));
        }

        if (ctx.methodBody() is { } body) {
            funcBody = ctx.Start.InputStream.GetText(new Interval(body.Start.StartIndex, body.Stop.StopIndex));
        }

        return new EnforceFunction {
            FunctionBody = funcBody,
            FunctionModifiers = funcModifiers,
            FunctionName = funcName,
            FunctionParameters = funcParameters,
            FunctionType = funcType
        };
    }

    public string ToEnforceFormat() {
        var enforce = new StringBuilder();
        foreach (var mod in FunctionModifiers) enforce.Append(mod).Append(' ');
        enforce.Append(FunctionType).Append(' ').Append(FunctionName).Append('(');
        var parametersString = string.Join(", ", FunctionParameters.Select(static fp => fp.ToEnforceFormat()));
        enforce.Append(parametersString).Append(')').Append(FunctionBody);
        return enforce.ToString();
    }
}