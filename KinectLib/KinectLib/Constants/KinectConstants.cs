using System.Windows.Media;

namespace KinectLib.Constants
{
    public static class KinectConstants
    {
        public const int BYTES_PER_PIXEL = 4;
        public const int DPI = 96;

        public const int BitmapWidth = 512;
        public const int BitmapHeight = 424;

        public const int ColorBitmapBytesPerPixel = 3;
        public static readonly PixelFormat ColorBitmapPixelFormat = PixelFormats.Bgr24;

        public const int DepthBitmapBytesPerPixel = 1;
        public static readonly PixelFormat DepthBitmapPixelFormat = PixelFormats.Gray8;
    }
}
