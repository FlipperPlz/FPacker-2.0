// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPacker.Antlr.Poseidon;
using FPacker.Formats.RAP.Models;

namespace FPacker.Formats.RAP.IO; 

public class RapPreParser : PoseidonBaseListener {
    public RapFile RapFile { get; protected set; }
    
    public override void ExitComputationalUnit(PoseidonParser.ComputationalUnitContext ctx) => RapFile = new RapFile().FromRapContext<RapFile>(ctx);
}