// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPacker.Formats.RAP.IO;

namespace FPacker.Formats.RAP.Models; 

public interface IRapBinarizable<out TSelf> {
    public TSelf FromBinaryContext(RapBinaryReader reader, bool defaultFalse = false);
    public IEnumerable<byte> ToBinaryContext() => throw new NotImplementedException();
}