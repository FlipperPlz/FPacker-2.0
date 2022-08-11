// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPackerLibrary.Antlr.Enforce;
using FPackerLibrary.Formats.Enforce.Models;

namespace FPackerLibrary.Formats.Enforce.Parse; 

public class EnforcePreParser : EnforceParserBaseListener {
    public EnforceScript Script;

    public override void ExitComputationalUnit(EnforceParser.ComputationalUnitContext context) =>
        Script = EnforceScript.FromContext(context);
}