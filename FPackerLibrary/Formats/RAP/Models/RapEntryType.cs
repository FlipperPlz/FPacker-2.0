// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

namespace FPackerLibrary.Formats.RAP.Models.Enums; 

public enum RapEntryType : byte {
    RapClass = 0,
    RapValue = 1,
    RapArray = 2,
    RapExternClass = 3,
    RapDeleteClass = 4
}