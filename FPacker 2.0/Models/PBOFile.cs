// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

using FPacker.Formats.RAP.Models;
using FPacker.Formats.RAP.Models.Values;
using FPacker.Models.AddonFiles;
using FPacker.PBO.Enums;
using FPacker.PBO.Models;

namespace FPacker.Models; 

public class PBOFile {
    public List<AddonPrefix> Prefixes = new();
    private readonly List<PBOEntry> _entries = new();
    private readonly List<ConfigFile> _configs = new();
    private readonly List<EnforceFile> _scripts = new();
    
    
    
    public void CreatePrefix(AddonPrefix prefix) {
        var prefixCfg = new RapFile { GlobalClasses = new List<RapClass> { new() { ClassName = "CfgPatches", ChildClasses = new List<RapClass>() { new() {ClassName = prefix.PrefixName, VariableStatements = new List<RapVariableStatement>() { new() {VariableName = "requiredAddons", VariableValue = new RapArray(new[] {"DZ_Data"})}}}}}}};
        var r = new ConfigFile() {
            PBOPath = prefix.PBOPath,
            PBOReferencePath = prefix.PrefixName + Path.DirectorySeparatorChar,
            ObjectBase = prefixCfg
        };
        _entries.Add(new PBOEntry( $"{prefix.PBOPath}\\config.cpp", (int) EntryPackingType.Uncompressed, EntryDataType.BinarizedPoseidonConfig, prefixCfg.BinarizedData(), ""));
        AddonPacker.Logger.Info("Prefix {prefix} was created at {loc}", prefix.PrefixName, prefix.PBOPath);
    }
    
    

}