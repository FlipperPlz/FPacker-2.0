namespace FPackerLibrary.P3D.Models.ODOL; 

public class AnimationRTWeight : VerySmallArray {
    public AnimationRTPair[] AnimationRTPairs
    {
        get
        {
            AnimationRTPair[] array = new AnimationRTPair[this.NSmall];
            for (int i = 0; i < this.NSmall; i++)
            {
                array[i] = new AnimationRTPair(this.SmallSpace[i * 2], this.SmallSpace[i * 2 + 1]);
            }
            return array;
        }
    }
}