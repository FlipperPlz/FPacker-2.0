// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models.Enums;
using FPacker.Formats.RAP.Models.Values;

namespace FPacker.Formats.RAP.Models.Parse; 

public class RapBinaryParser : IDisposable {
    private readonly RapBinaryReader? _reader;
    public RapFile ParsedRapFile = new();

    private uint _enumOffset;
    private int _parentEntryCount;

    public RapBinaryParser(string filepath) {
        _reader = new RapBinaryReader(File.ReadAllBytes(filepath));
        ParseConfig();
    }

    private void ParseConfig() {
        if (_reader is null) throw new NullReferenceException("_reader is null, cannot continue");
        if (!_reader.ReadHeader()) throw new Exception("An invalid raP file was passed to RapBinaryParser.");
        if (_reader.IsOperationFlashpointFormat()) throw new Exception("Operation Flashpoint is not supported.");
        if(_reader.ReadUInt32() != 0 || _reader.ReadUInt32() != 8) throw new Exception("Expected bytes 0 and 8.");

        _enumOffset = _reader.ReadUInt32();
        if (!ReadParentClasses()) Console.WriteLine("No parent classes were found.");
        if (!ReadChildClasses()) Console.WriteLine("No child classes were found.");
    }

    private bool ReadParentClasses() {
        _reader.ReadAsciiZ();
        _parentEntryCount = _reader.ReadCompressedInteger();

        for (var i = 0; i < _parentEntryCount; ++i) {
            switch ((RapEntryType) _reader.ReadByte()) {
                case RapEntryType.RapClass:
                    ParsedRapFile.GlobalClasses.Add(_reader.ReadBinarizedEntry<RapClass>());
                    break;
                case RapEntryType.RapExternClass:
                    ParsedRapFile.GlobalClasses.Add(_reader.ReadBinarizedEntry<RapClass>(true));
                    break;
                case RapEntryType.RapValue:
                    ParsedRapFile.GlobalVariableStatements.Add(_reader.ReadBinarizedEntry<RapVariableStatement>());
                    break;
                case RapEntryType.RapDeleteClass:
                    ParsedRapFile.GlobalDeleteStatements.Add(_reader.ReadBinarizedEntry<RapDeleteStatement>());
                    break;
                case RapEntryType.RapArray:
                    ParsedRapFile.GlobalVariableStatements.Add(_reader.ReadBinarizedEntry<RapVariableStatement>(true));
                    break;
                default: throw new NotImplementedException();
            }
            
        }

        return ParsedRapFile.GlobalClasses.Count > 0;
    }
    
    private bool ReadChildClasses() {
        ParsedRapFile.GlobalClasses.ForEach(LoadChildClasses);
        return ParsedRapFile.GlobalClasses.Count > 0;
    }

    private void LoadChildClasses(RapClass child) {
        if(child.ExternalClass) return;
        
        _reader.Position = child.BinaryOffset;

        var parent = _reader.ReadAsciiZ();
        child.ParentClass = (parent == string.Empty) ? null : parent;
        child.EntryCount = _reader.ReadCompressedInteger();

        for (var i = 0; i < child.EntryCount; ++i) AddEntryToClass(child);

        child.ChildClasses.ForEach(LoadChildClasses);
    }

    private void AddEntryToClass(RapClass child) {
        if(child.ExternalClass) return;

        var entryType = (RapEntryType)_reader.ReadByte();

        switch (entryType) {
            case RapEntryType.RapArray:
                child.VariableStatements.Add(_reader.ReadBinarizedEntry<RapVariableStatement>(true));
                break;
            case RapEntryType.RapValue:
                child.VariableStatements.Add(_reader.ReadBinarizedEntry<RapVariableStatement>());
                break;
            case RapEntryType.RapDeleteClass:
                child.DeleteStatements.Add(_reader.ReadBinarizedEntry<RapDeleteStatement>());
                break;
            case RapEntryType.RapExternClass:
                child.ChildClasses.Add(_reader.ReadBinarizedEntry<RapClass>(true));
                break;
            case RapEntryType.RapClass:
                child.ChildClasses.Add(_reader.ReadBinarizedEntry<RapClass>());
                break;
            default:
                throw new Exception(entryType.ToString());
        }
    }
    
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing) {
        if (!disposing) return;
        _reader?.Dispose();
    }
}