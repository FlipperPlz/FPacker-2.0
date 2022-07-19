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
        int value;
        if ((value = ReadByte()) == 0) {
            return 0;
        }

        while ((value & 0x80) != 0) {
            int extra;
            if ((extra = ReadByte()) == 0) return 0;

            value += (extra - 1) * 0x80;
        }

        return value;
        
    }

    
    //TODO:Move to BaseValueType
    public RapString ReadRapString() => new(ReadAsciiZ());
    public RapInt ReadRapInt() => new(ReadInt32());
    public RapFloat ReadRapFloat() => new(ReadSingle());
    public RapArray ReadRapArray() {
        var output = new RapArray() {
            EntryCount = ReadCompressedInteger()
        };
        if (output.EntryCount == 0) return output;

        for (var i = 0; i < output.EntryCount; ++i) {
            switch ((RapValueType) ReadByte()) {
                case RapValueType.String:
                    output.Value.Add(ReadRapString());
                    break;
                case RapValueType.Float:
                    output.Value.Add(ReadRapFloat());
                    break;
                case RapValueType.Long:
                    output.Value.Add(ReadRapInt());
                    break;
                case RapValueType.Array:
                    output.Value.Add(ReadRapArray());
                    break;
                case RapValueType.Variable: 
                    output.Value.Add(ReadRapArray());
                    break;
                default: throw new Exception("How the fuck did you get here?");
            }
        }

        return output;
    }

    public TRapEntry ReadBinarizedEntry<TRapEntry>(bool defaultFalse = false) where TRapEntry : class, IRapBinarizable<TRapEntry>, new() =>
        new TRapEntry().FromBinaryContext(this, defaultFalse);
    
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