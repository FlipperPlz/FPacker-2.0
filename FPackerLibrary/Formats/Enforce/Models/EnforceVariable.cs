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

public class EnforceVariable : IEnforceDeserializable<EnforceParser.FieldDeclarationContext, EnforceVariable>, IEnforceSerializable {
    public List<string> VariableModifiers { get; set; }
    public string VariableType { get; set; }
    public Dictionary<string, string> DefinedVariables { get; set; }
    
    public EnforceVariable FromEnforceContext(EnforceParser.FieldDeclarationContext ctx) => FromContext(ctx);

    public static EnforceVariable FromContext(EnforceParser.FieldDeclarationContext ctx) {
        var fieldModifiers = new List<string>();
        var fieldType = "";
        var fieldsDefined = new Dictionary<string, string>();

        //Identify Modifiers
        if (ctx.variableModifier() is { } modifierCtx) {
            fieldModifiers.AddRange(modifierCtx.Select(mod => ctx.Start.InputStream.GetText(new Interval(mod.Start.StartIndex, mod.Stop.StopIndex))));
        }

        //Identify Type
        if (ctx.typeType() is { } typeCtx) {
            fieldType = ctx.Start.InputStream.GetText(new Interval(typeCtx.Start.StartIndex, typeCtx.Stop.StopIndex));
        }

        //Identify Fields & Values
        if (ctx.variableDeclarators() is not { } varDeclarators || varDeclarators.variableDeclarator() is not { } varDeclaratorsList) return new EnforceVariable() {
                VariableModifiers = fieldModifiers,
                VariableType = fieldType,
                DefinedVariables = fieldsDefined
            };
        foreach (var varDeclarator in varDeclaratorsList) {
            fieldsDefined.Add(
                (varDeclarator.variableDeclaratorId() is { } variableDeclaratorId)
                    ? ctx.Start.InputStream.GetText(new Interval(variableDeclaratorId.Start.StartIndex, variableDeclaratorId.Stop.StopIndex))
                    : "",
                (varDeclarator.variableInitializer() is { } variableInitializer)
                    ? ctx.Start.InputStream.GetText(new Interval(variableInitializer.Start.StartIndex, variableInitializer.Stop.StopIndex))
                    : "");
        }

        return new EnforceVariable() {
            VariableModifiers = fieldModifiers,
            VariableType = fieldType,
            DefinedVariables = fieldsDefined
        };
    }

    public string ToEnforceFormat() {
        var enforce = new StringBuilder();
        foreach (var (fieldName, fieldValue) in DefinedVariables) {
            foreach (var mod in VariableModifiers) enforce.Append(mod).Append(' ');
            enforce.Append(VariableType).Append(' ').Append(fieldName);

            if (fieldValue != "") enforce.Append(" = ").Append(fieldValue).Append(';').Append('\n');
            
        }
        return enforce.ToString();
    }
}