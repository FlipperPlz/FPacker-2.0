using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using FPacker.P3D.Models.MLOD;
using FPacker.P3D.Models.MLOD.Tagg;
using FPacker.P3D.Models.ODOL;

namespace FPacker.P3D.Models; 

public class P3DFile {
    public string PBOPath { get; private set; }
    public string PBOReferencePath { get; private set; }
    public string SystemPath { get; init; }
    
    public P3D P3DData { get; private set; }
    
    public List<string> MaterialPaths { get; private set; } = new();
    public List<string> TexturePaths { get; private set; } = new();
    public List<string> SurfacePaths { get; private set; } = new();
    
    public List<string> ProxyPaths { get; private set; } = new();
    
    public List<string> FoundPaths { get; private set; } = new();

    public Dictionary<string, string> ObfuscatedPaths { get; set; } = new();
    
    
    public P3DFile(string pboPath, string pboRefPath, string systemPath) {
        PBOPath = pboPath;
        PBOReferencePath = pboRefPath;
        SystemPath = systemPath;
        ParseP3D();
    }

    private void ParseP3D() {
        P3DData = P3D.GetInstance(SystemPath);
        switch (P3DData) {
            case ODOL.ODOL odolP3d:
                throw new NotImplementedException("FPacker does not support the obfuscation of binarized models due to theft.");
                foreach (var odolLod in odolP3d.LODs) {
                    foreach (var odolLodMaterialName in odolLod.MaterialNames) {
                        var matName = odolLodMaterialName.TrimStart('"').TrimEnd('"');
                        if(!MaterialPaths.Contains(matName) && !matName.StartsWith('#')) MaterialPaths.Add(matName);

                    }

                    foreach (var odolLodTextureName in odolLod.Textures) {
                        var textureName = odolLodTextureName.TrimStart('"').TrimEnd('"');
                        if(textureName != string.Empty && !textureName.StartsWith('#') && !TexturePaths.Contains(textureName)) TexturePaths.Add(textureName);
                    }

                    if(odolLod is not LOD lod) continue;
                    foreach (var embeddedMaterial in lod.Materials) {
                        var matName = embeddedMaterial.MaterialName.TrimStart('"').TrimEnd('"');
                        var surfaceName = embeddedMaterial.SurfaceFile.TrimStart('"').TrimEnd('"');

                        if(matName != string.Empty && !MaterialPaths.Contains(matName) && !matName.StartsWith('#'))
                            MaterialPaths.Add(matName);
                        if(surfaceName != string.Empty && !SurfacePaths.Contains(surfaceName) && !surfaceName.StartsWith('#')) SurfacePaths.Add(surfaceName);
                        
                        foreach (var stageTexture in embeddedMaterial.StageTextures) {
                            var textureName = stageTexture.Texture.TrimStart('"').TrimEnd('"');
                            if (textureName != string.Empty && !textureName.StartsWith('#') && !TexturePaths.Contains(textureName)) TexturePaths.Add(textureName);
                        }
                        
                    }
                }
                break;
            case MLOD.MLOD mlodP3d:
                foreach (var odolLod in mlodP3d.LODs) {
                    foreach (var face in ((MLOD_LOD)odolLod).Faces) {
                        var textureName = face.Texture;
                        var matName = face.Material;
                        if (matName != string.Empty && !matName.StartsWith('#') && !MaterialPaths.Contains(matName)) MaterialPaths.Add(matName);
                        if (textureName != string.Empty && !textureName.StartsWith('#') && !TexturePaths.Contains(textureName)) TexturePaths.Add(textureName);
                    }

                    var mlodLOD = odolLod as MLOD_LOD;
                    foreach (var lodTagg in mlodLOD.Taggs) {
                        if(!lodTagg.Name.ToLower().StartsWith("proxy")) continue;
                        var proxyPath = lodTagg.Name[7..];
                        if (proxyPath != string.Empty && !ProxyPaths.Contains(proxyPath)) ProxyPaths.Add(proxyPath);
                    }
                }
                break;
        }
        FoundPaths.AddRange(MaterialPaths);
        FoundPaths.AddRange(SurfacePaths);
        FoundPaths.AddRange(TexturePaths);
    }
    
    
    public void ObfuscatePaths(string obfuscatedPboPath, string obfuscatedPboRefPath) {
        PBOPath = obfuscatedPboPath;
        PBOReferencePath = obfuscatedPboRefPath;
        switch (P3DData) {
            case ODOL.ODOL odolP3d:
                throw new NotImplementedException("FPacker does not support the obfuscation of binarized models due to theft.");
                foreach (var odolLod in odolP3d.LODs) {
                    if(odolLod is not LOD lod) continue;
                    foreach (var embeddedMaterial in lod.Materials) {
                        var materialFile = embeddedMaterial.MaterialName.TrimStart('"').TrimEnd('"');
                        var surfaceFile = embeddedMaterial.SurfaceFile.TrimStart('"').TrimEnd('"');
                        
                        if (ObfuscatedPaths.ContainsKey(materialFile)) embeddedMaterial.MaterialName = ObfuscatedPaths[materialFile];
                        if (ObfuscatedPaths.ContainsKey(surfaceFile)) embeddedMaterial.MaterialName = ObfuscatedPaths[surfaceFile];
                        
                        foreach (var stageTexture in embeddedMaterial.StageTextures) {
                            var textureName = stageTexture.Texture.TrimStart('"').TrimEnd('"');
                            if (ObfuscatedPaths.ContainsKey(textureName)) stageTexture.Texture = ObfuscatedPaths[textureName];
                        }
                    }
                }
                break;
            case MLOD.MLOD mlodP3d:
                foreach (var lod in mlodP3d.LODs) {
                    if (lod is not MLOD_LOD mlodLod) continue;
                    foreach (var face in mlodLod.Faces) {
                        var textureName = face.Texture;
                        var materialFile = face.Material;

                        if (textureName != string.Empty && ObfuscatedPaths.TryGetValue(textureName, out var obfTextureName)) {
                            face.Texture = obfTextureName;
                        }

                        if (materialFile != string.Empty && ObfuscatedPaths.TryGetValue(materialFile, out var obfMaterialFile)) {
                            face.Material = obfMaterialFile;
                        }
                    }

                    foreach (var tagg in mlodLod.Taggs) {
                        if(!tagg.Name.ToLower().StartsWith("proxy:\\")) continue;
                        try { tagg.Name = "proxy:\\" + ObfuscatedPaths[tagg.Name[7..]]; } catch (Exception e) { }
                    }
                }
                break;
        }
    }
}