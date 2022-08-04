namespace FPackerLibrary.P3D.Models.ODOL; 

public class AnimationRTPair {
    public byte SelectionIndex { get; }
    public byte Weight { get; }
    
    public AnimationRTPair(byte sel, byte weight) {
        this.SelectionIndex = sel;
        this.Weight = weight;
    }
}