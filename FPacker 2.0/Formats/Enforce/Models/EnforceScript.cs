// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using System.Text;
using FPacker.Antlr.Enforce;

namespace FPacker.Formats.Enforce.Models;

public class EnforceScript : IEnforceDeserializable<EnforceParser.ComputationalUnitContext, EnforceScript>, IEnforceSerializable {
    public List<EnforceVariable> GlobalFields { get; set; }
    public List<EnforceFunction> GlobalFunctions { get; set; }
    public List<EnforceClass> Classes { get; set; }

    public EnforceScript FromEnforceContext(EnforceParser.ComputationalUnitContext ctx) => FromContext(ctx);

    public static EnforceScript FromContext(EnforceParser.ComputationalUnitContext ctx) {
        var globalFields = new List<EnforceVariable>();
        var globalFunctions = new List<EnforceFunction>();

        foreach (var globalDeclarationContext in ctx.globalDeclaration()) {
            if (globalDeclarationContext.fieldDeclaration() is not null) {
                globalFields.Add(EnforceVariable.FromContext(globalDeclarationContext.fieldDeclaration()));
            } else if (globalDeclarationContext.methodDeclaration() is not null) {
                globalFunctions.Add(EnforceFunction.FromContext(globalDeclarationContext.methodDeclaration()));
            }
        }

        var globalClasses = (from typeDeclarationContext in ctx.typeDeclaration() where typeDeclarationContext.classDeclaration() is not null select EnforceClass.FromContext(typeDeclarationContext.classDeclaration())).ToList();

        return new EnforceScript {
            GlobalFields = globalFields,
            GlobalFunctions = globalFunctions,
            Classes = globalClasses
        };
    }

    public string ToEnforceFormat() {
        StringBuilder scriptBuilder = new();
        GlobalFields.ForEach(gf => scriptBuilder.Append(gf.ToEnforceFormat()).Append('\n'));
        GlobalFunctions.ForEach(gf => scriptBuilder.Append(gf.ToEnforceFormat()).Append('\n'));
        Classes.ForEach(c => scriptBuilder.Append(c.ToEnforceFormat()).Append('\n'));
        return scriptBuilder.ToString();
    }

    
}