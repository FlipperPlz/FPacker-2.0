// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPacker.Antlr.Enforce;
using FPacker.Formats.Enforce.Models;

namespace FPacker.Formats.Enforce.Parse; 

public class EnforcePreParser : EnforceParserBaseListener {
    public EnforceScript Script;

    public override void ExitComputationalUnit(EnforceParser.ComputationalUnitContext context) =>
        Script = EnforceScript.FromContext(context);
}