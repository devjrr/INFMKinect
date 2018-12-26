using System;

namespace KinectLib.Classes
{
    public interface IColorPointCloud
    {
        DateTime TimeStamp { get; set; }

        int Width { get; set; }

        int Height { get; set; }

        byte[] DisplayPixels { get; set; }

        ushort[] DepthData { get; set; }

        byte[] ColorData { get; set; }

        bool[] BodyData { get; set; }
    }
}
