// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using System.Text;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.IO; 

public class RapBinaryReader : BinaryReader {
    public long Position {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }
    public bool HasReachedEnd => BaseStream.Position == BaseStream.Length;
    
    public RapBinaryReader(Stream stream) : base(stream) { }
    public RapBinaryReader(byte[] binaryData) : base(new MemoryStream(binaryData, false)) { }

    public int ReadCompressedInteger() {
        var value = 0;
        for (var i = 0;; ++i) {
            var v = ReadByte();
            value |= v & 0x7F << (7 * i);
            if((v & 0x80) == 0) break;
        }

        return value;
    }
    
    
    public TRapValue ReadBinarizedValue<TRapValue>() where TRapValue : class, IRapDeserializable, new() =>
        new TRapValue().FromBinaryContext<TRapValue>(this);

    public TRapEntry ReadBinarizedEntry<TRapEntry>(bool defaultFalse = false) where TRapEntry : class, IRapDeserializable, new() =>
        new TRapEntry().FromBinaryContext<TRapEntry>(this, defaultFalse);
    
    public bool ReadHeader() {
        var bits = ReadBytes(4);
        return bits[0] == '\0' && bits[1] == 'r' && bits[2] == 'a' && bits[3] == 'P';
    }
    
    public bool IsOperationFlashpointFormat() {
        var pos = BaseStream.Position;
        BaseStream.Position = 5;
        var bits = ReadBytes(4);

        // 04\0\0
        var isOfp = bits[0] == '0' && bits[1] == '4' && bits[2] == '\0' && bits[3] == '\0';

        // Reset position
        BaseStream.Position = ((uint)pos);
        return isOfp;
    }
    
    public string ReadAsciiZ() {
        var builder = new StringBuilder();
        char c;
        while ((c = (char)ReadByte()) != '\0') builder.Append(c);
        return builder.ToString();
    }
}