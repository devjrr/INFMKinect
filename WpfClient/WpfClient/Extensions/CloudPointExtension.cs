using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectLib.Constants;
using NetClientLib;

namespace WpfClient.Extensions
{
    public static class CloudPointExtension
    {
        public static BitmapSource GenerateColorBitmap(this IList<CloudPoint> iCloudPoint)
        {
            const int width = 256;
            const int height = 212;
            var pixelFormat = PixelFormats.Rgb24;
            const int bytesPerPixel = 3;
            const int stride = bytesPerPixel * width;

            var buffer = new byte[width * height * bytesPerPixel];


            foreach (var p in iCloudPoint)
            {
                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel] = (byte)(p.GetR() * 256);
                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel + 1] = (byte)(p.GetG() * 256);
                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel + 2] = (byte)(p.GetB() * 256);
            }

            return BitmapSource.Create(width, height, KinectConstants.DPI, KinectConstants.DPI, pixelFormat, null, buffer, stride);
        }

        public static BitmapSource GenerateDepthBitmap(this IList<CloudPoint> iCloudPoint)
        {
            const int width = 256;
            const int height = 212;
            var pixelFormat = PixelFormats.Gray16;
            const int bytesPerPixel = 2;
            const int stride = bytesPerPixel * width;

            var buffer = new ushort[width * height];

            foreach (var p in iCloudPoint)
            {
                buffer[width * (int)p.GetY() + (int)p.GetX()] = (ushort)(p.GetZ() * 256);
            }

            return BitmapSource.Create(width, height, KinectConstants.DPI, KinectConstants.DPI, pixelFormat, null, buffer, stride);
        }

        public static BitmapSource GenerateDepthBitmap(this ushort[] iDepthArray, bool[] iBodyArray)
        {
            const int stride = KinectConstants.DepthBitmapBytesPerPixel * KinectConstants.BitmapWidth;

            var buffer = new byte[KinectConstants.BitmapWidth * KinectConstants.BitmapHeight];

            for (var i = 0; i < iDepthArray.Length; i++)
            {
                buffer[i] = iBodyArray[i] ? (byte)iDepthArray[i] : byte.MinValue;
            }

            return BitmapSource.Create(KinectConstants.BitmapWidth, KinectConstants.BitmapHeight, KinectConstants.DPI, KinectConstants.DPI, KinectConstants.DepthBitmapPixelFormat, null, buffer, stride);
        }

        public static BitmapSource GenerateColorBitmap(this byte[] iColorArray, bool[] iBodyArray)
        {
            const int stride = KinectConstants.ColorBitmapBytesPerPixel * KinectConstants.BitmapWidth;

            return BitmapSource.Create(KinectConstants.BitmapWidth, KinectConstants.BitmapHeight, KinectConstants.DPI, KinectConstants.DPI, KinectConstants.ColorBitmapPixelFormat, null, iColorArray, stride);
        }
    }
}
