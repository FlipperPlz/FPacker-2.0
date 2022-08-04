// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

namespace FPackerLibrary.PBO.Models; 

public class AddonFile {
    public string PBOPath { get; init; }
    public string PBOReferencePath { get; init; }
    public string ObfuscatedPBOPath { get; private set; }
    public string ObfuscatedPBORefPath { get; private set; }
    public string SystemPath { get; init; }
}