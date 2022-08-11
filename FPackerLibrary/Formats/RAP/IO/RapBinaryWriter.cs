// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPackerLibrary.Formats.RAP.Models;
using FPackerLibrary.Formats.RAP.Models.Values;

namespace FPackerLibrary.Formats.RAP.IO; 

public class RapBinaryWriter : BinaryWriter {
    public long Position {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }
    public RapBinaryWriter(Stream stream) : base(stream) { }

    public void WriteCompressedInt(int data) {
        do {
            var current = data % 0x80;
            data = (int) Math.Floor((decimal) (data / 0x80));
            if (data is not 0) current = current | 0x80; 
            Write((byte) current);
        } while (data > 0x7F);

        if (data is not 0) {
            Write((byte) data);
        }
    }
    
    public void WriteAsciiZ(string text = "") {
        Write(text.ToCharArray());
        Write(char.MinValue);
    }

    public void WriteBinarizedValue<TValue>(TValue value) where TValue : IRapDeserializable, IRapValue =>
        value.ToBinaryContext(this, false);

    public void WriteBinarizedEntry<TEntry>(TEntry entry, bool defaultFalse = false) where TEntry : IRapDeserializable, IRapEntry =>
        entry.ToBinaryContext(this, defaultFalse);
}