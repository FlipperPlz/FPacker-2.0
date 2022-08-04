// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using System.Text;
using Antlr4.Runtime.Misc;
using FPacker.Antlr.Enforce;

namespace FPacker.Formats.Enforce.Models; 

public class EnforceFunctionParameter : IEnforceDeserializable<EnforceParser.FormalParameterContext, EnforceFunctionParameter> {
    public List<string> VariableModifiers { get; set; }
    public string VariableType { get; set; }
    public string VariableName { get; set; }
    public string? VariableDefaultValue { get; set; }
    
    public EnforceFunctionParameter FromEnforceContext(EnforceParser.FormalParameterContext ctx) => FromContext(ctx);

    public static EnforceFunctionParameter FromContext(EnforceParser.FormalParameterContext ctx) {
        var paramModifiers = new List<string>();
        var paramType = "";
        var paramName = "";
        string paramDefaultValue = null;

        if (ctx.formalParameterDefined() is { } formalParameterDefined) {
            //Identify Modifiers
            if(formalParameterDefined.parameterModifier() is { } modifierCtx) 
                paramModifiers.AddRange(modifierCtx.Select(static mod => mod.GetText()));

            //Identify Type
            if (formalParameterDefined.typeTypeOrVoid() is { } typeCtx)
                paramType = ctx.Start.InputStream.GetText(new Interval(typeCtx.Start.StartIndex, typeCtx.Stop.StopIndex));

            //Identify Name & Default Value
            if (formalParameterDefined.variableDeclarator() is { } varDeclarator) {
                //Identify Name
                if (varDeclarator.variableDeclaratorId() is { } varDeclaratorId) {
                    if (varDeclaratorId.identifier() is { } varIdentifier) {
                        paramName = ctx.Start.InputStream.GetText(new Interval(varIdentifier.Start.StartIndex, varIdentifier.Stop.StopIndex));
                    }
                }
                //Identify Default Value
                if (varDeclarator.variableInitializer() is { } variableInitializer) {
                    paramDefaultValue = ctx.Start.InputStream.GetText(new Interval(variableInitializer.Start.StartIndex, variableInitializer.Stop.StopIndex));
                }
            }

        } else if (ctx.formalParameterUndefined() is { } formalParameterUndefined) {
            //Identify Modifiers
            if(formalParameterUndefined.parameterModifier() is {} modifierContext)
                paramModifiers.AddRange(modifierContext.Select(mod => mod.GetText()));
            
            //Identify Type
            if (formalParameterUndefined.typeTypeOrVoid() is { } typeCtx)
                paramType = ctx.Start.InputStream.GetText(new Interval(typeCtx.Start.StartIndex, typeCtx.Stop.StopIndex));
            
            //Identify Name
            if (formalParameterUndefined.variableDeclaratorId() is { } varDeclaratorId) {
                if (varDeclaratorId.identifier() is { } varIdentifier) {
                    paramName = ctx.Start.InputStream.GetText(new Interval(varIdentifier.Start.StartIndex, varIdentifier.Stop.StopIndex));
                }
            }
        }

        return new EnforceFunctionParameter {
            VariableModifiers = paramModifiers,
            VariableType = paramType,
            VariableName = paramName,
            VariableDefaultValue = paramDefaultValue
        };
    }

    public string ToEnforceFormat() {
        var enforce = new StringBuilder();
        enforce.Append(string.Join(' ', VariableModifiers)).Append(' ');
        enforce.Append(VariableType).Append(' ').Append(VariableName);
        if (VariableDefaultValue is not null) enforce.Append(" = ").Append(VariableDefaultValue);
        return enforce.ToString();
    }
}