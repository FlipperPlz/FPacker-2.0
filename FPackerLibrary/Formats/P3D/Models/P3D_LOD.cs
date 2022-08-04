using FPacker.P3D.Math;

namespace FPacker.P3D.Models; 

public abstract class P3D_LOD {
    public string Name => this.resolution.getLODName();
    public float Resolution => this.resolution;
    
    public abstract Vector3P[] Points { get; set; }
    public abstract Vector3P[] Normals { get; set; }
    public abstract string[] Textures { get; }
    public abstract string[] MaterialNames { get; }

    protected float resolution;
}