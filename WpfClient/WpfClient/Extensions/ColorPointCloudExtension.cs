using KinectLib.Classes;
using LightBuzz.Vitruvius;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfClient.Extensions
{
    public static class ColorPointCloudExtension
    {
        public static WriteableBitmap GenerateImage(this ColorPointCloud iColorPointCloud)
        {
            var bmp = new WriteableBitmap(iColorPointCloud.Width, iColorPointCloud.Height, Constants.DPI, Constants.DPI, PixelFormats.Bgr24, null);

            bmp.Lock();

            Marshal.Copy(iColorPointCloud.ColorData, 0, bmp.BackBuffer, iColorPointCloud.ColorData.Length);
            bmp.AddDirtyRect(new Int32Rect(0, 0, iColorPointCloud.Width, iColorPointCloud.Height));

            bmp.Unlock();

            return bmp;
        }
    }
}
