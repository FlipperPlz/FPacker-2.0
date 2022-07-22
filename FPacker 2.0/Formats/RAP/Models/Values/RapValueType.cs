// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

namespace FPacker.Formats.RAP.Models.Enums; 

public enum RapValueType : byte {
    String = 0,
    Float = 1,
    Long = 2,
    Array = 3,
    Variable = 4
}