﻿// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

namespace FPacker.Models; 

public abstract class BaseAddonFile {
    public string PBOPath { get; set; }
    public string PBOReferencePath { get; init; }
    public string ObfuscatedPBOPath { get; protected internal set; }
    public string ObfuscatedPBORefPath { get; protected internal set; }
    public string SystemPath { get; init; }
    public abstract IEnumerable<byte> Data { get; set; }

    public BaseAddonFile(string pboPath, string pboRefPath, string systemPath) {
        PBOPath = pboPath;
        PBOReferencePath = pboRefPath;
        SystemPath = systemPath;
    }
}