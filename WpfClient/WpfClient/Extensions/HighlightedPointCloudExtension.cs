using KinectLib.Classes;
using LightBuzz.Vitruvius;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfClient.Extensions
{
    public static class HighlightedPointCloudExtension
    {
        public static WriteableBitmap GenerateImage(this HighlightedPointCloud iHighlightedPointCloud)
        {
            if (iHighlightedPointCloud == null) return null;

            var bmp = new WriteableBitmap(iHighlightedPointCloud.Width, iHighlightedPointCloud.Height, Constants.DPI, Constants.DPI, Constants.FORMAT, null);
            var highlightedPixels = new byte[iHighlightedPointCloud.Width * iHighlightedPointCloud.Height * Constants.BYTES_PER_PIXEL];

            // Convert the depth to RGB
            for (int depthIndex = 0, colorPixelIndex = 0; depthIndex < iHighlightedPointCloud.DepthData.Length && colorPixelIndex < highlightedPixels.Length; depthIndex++, colorPixelIndex += 4)
            {
                // Get the depth for this pixel
                var depth = iHighlightedPointCloud.DepthData[depthIndex];
                var player = iHighlightedPointCloud.BodyData[depthIndex];

                // To convert to a byte, we're discarding the most-significant
                // rather than least-significant bits.
                // We're preserving detail, although the intensity will "wrap."
                // Values outside the reliable depth range are mapped to 0 (black).
                var intensity = (byte)(depth >= iHighlightedPointCloud.MinDepth && depth <= iHighlightedPointCloud.MaxDepth ? depth : 0);

                if (player != 0xff)
                {
                    // Color player gold.
                    highlightedPixels[colorPixelIndex + 0] = (byte)((intensity * Colors.Red.B) / byte.MaxValue); // B
                    highlightedPixels[colorPixelIndex + 1] = (byte)((intensity * Colors.Red.G) / byte.MaxValue); // G
                    highlightedPixels[colorPixelIndex + 2] = (byte)((intensity * Colors.Red.R) / byte.MaxValue); // R
                }
                else
                {
                    // Color the rest of the image in grayscale.
                    highlightedPixels[colorPixelIndex + 0] = 0x00; // B
                    highlightedPixels[colorPixelIndex + 1] = 0x00; // G
                    highlightedPixels[colorPixelIndex + 2] = 0x00; // R
                }
            }

            bmp.Lock();

            Marshal.Copy(highlightedPixels, 0, bmp.BackBuffer, highlightedPixels.Length);
            bmp.AddDirtyRect(new Int32Rect(0, 0, iHighlightedPointCloud.Width, iHighlightedPointCloud.Height));

            bmp.Unlock();

            return bmp;
        }
    }
}
