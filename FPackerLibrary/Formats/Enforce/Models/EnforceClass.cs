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

//TODO: Type Support
public class EnforceClass : IEnforceDeserializable<EnforceParser.ClassDeclarationContext, EnforceClass>, IEnforceSerializable {
    public bool ModdedClass { get; set; }
    public string ClassName { get; set; }
    public string? ParentClass { get; set; }
    public List<EnforceVariable> Fields { get; set; }
    public List<EnforceFunction> Functions { get; set; }

    public EnforceClass FromEnforceContext(EnforceParser.ClassDeclarationContext ctx) => FromContext(ctx);

    public static EnforceClass FromContext(EnforceParser.ClassDeclarationContext ctx) {
        var modded = false;
        var className = "";
        string parentClass = null;
        var fields = new List<EnforceVariable>();
        var functions = new List<EnforceFunction>();

        if (ctx.Parent is EnforceParser.TypeDeclarationContext typeDeclarationContext) {
            modded = typeDeclarationContext.MODDED() is not null;
        }

        if (ctx.identifier() is { } identifierContext) {
            className = ctx.Start.InputStream.GetText(new Interval(identifierContext.Start.StartIndex, identifierContext.Stop.StopIndex));
        }

        if (ctx.classOrEnumExtension() is { } classOrEnumExtensionContext) {
            if (classOrEnumExtensionContext.typeType() is { } typeTypeContext) {
                parentClass = ctx.Start.InputStream.GetText(new Interval(typeTypeContext.Start.StartIndex, typeTypeContext.Stop.StopIndex));
            }
        }


        if (ctx.classBody() is { } classBodyContext 
            && classBodyContext.globalDeclaration() is { } globalDeclarationContexts) {
            foreach (var globalDeclaration in globalDeclarationContexts) {
                if (globalDeclaration.fieldDeclaration() is { } fieldDeclarationContext) {
                    fields.Add(EnforceVariable.FromContext(fieldDeclarationContext));
                } else if (globalDeclaration.methodDeclaration() is { } methodDeclarationContext) {
                    functions.Add(EnforceFunction.FromContext(methodDeclarationContext));
                }
            }
        }

        return new EnforceClass {
            ModdedClass = modded,
            ClassName = className,
            ParentClass = parentClass,
            Fields = fields,
            Functions = functions
        };
    }

    public string ToEnforceFormat() {
        var builder = new StringBuilder();
        if (ModdedClass) builder.Append("modded ");
        builder.Append("class ");
        builder.Append(ClassName).Append(' ');
        if (ParentClass is not null) builder.Append(" : ").Append(ParentClass);
        builder.Append('{').Append('\n');
        Fields.ForEach(f => builder.Append(f.ToEnforceFormat()).Append('\n'));
        Functions.ForEach(f => builder.Append(f.ToEnforceFormat()).Append('\n'));
        builder.Append('}');
        return builder.ToString();
    }
}