// /*******************************************************
//  * Copyright (C) 2021-2022 Ryann (Elijah Cyr) <elijahcyr@protonmail.com>
//  *
//  *
//  * FPacker 2.0 can not be copied and/or distributed without the express
//  * permission of Ryann
//  *******************************************************/

namespace FPacker.Models; 

public abstract class BaseAddonFileSerializable<TObj> : BaseAddonFile {
    public TObj ObjectBase;
    
    protected BaseAddonFileSerializable(string pboPath, string pboRefPath, string systemPath) : base(pboPath, pboRefPath,
        systemPath) {
        InitializeObject(File.OpenRead(systemPath));
    }
    public BaseAddonFileSerializable() { }
    protected abstract void InitializeObject(Stream stream);
    public abstract void ObfuscateObject();
}