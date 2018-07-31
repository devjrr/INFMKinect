using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfClient.Extensions
{
    public static class DrawingImageExtension
    {
        public static BitmapSource ToBitmapSource(this DrawingImage source)
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new Point(0, 0), new Size(source.Width, source.Height)));
            drawingContext.Close();

            var bmp = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }
    }
}
