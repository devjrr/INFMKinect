using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using KinectLib.Classes;
using WpfClient.Classes;

namespace WpfClient.Extensions
{
    public static class DrawingGroupExtenstion
    {
        private static readonly SkeletonBase SkeletonBase = new SkeletonBase();

        public static ImageSource GenerateImage(this DrawingGroup drawingGroup, BodyWrapper body, CoordinateMapper coordinateMapper, FrameDescription frameDescription)
        {
            using (var dc = drawingGroup.Open())
            {
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, frameDescription.Width, frameDescription.Height));

                var drawPen = new Pen(Brushes.Red, 6);
                SkeletonBase.DrawSkeleton(body, drawPen, coordinateMapper, dc);

                drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, frameDescription.Width, frameDescription.Height));
            }

            return new DrawingImage(drawingGroup);
        }
    }
}
