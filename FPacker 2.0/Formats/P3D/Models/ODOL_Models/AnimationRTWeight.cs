namespace FPacker.P3D.Models.ODOL; 

public class AnimationRTWeight : VerySmallArray {
    public IEnumerable<AnimationRTPair> AnimationRTPairs
    {
        get
        {
            var array = new AnimationRTPair[this.NSmall];
            for (var i = 0; i < this.NSmall; i++)
            {
                array[i] = new AnimationRTPair(this.SmallSpace[i * 2], this.SmallSpace[i * 2 + 1]);
            }
            return array;
        }
    }
}