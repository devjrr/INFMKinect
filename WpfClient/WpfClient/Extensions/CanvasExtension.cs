using System.IO;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KinectLib.Interfaces;

namespace WpfClient.Extensions
{
    public static class CanvasExtension
    {

        private static readonly Brush HandClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
        private static readonly Brush HandOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
        private static readonly Brush HandLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        public static Joint ScaleTo(this Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public static Joint ScaleTo(this Joint joint, double width, double height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            var value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel)
            {
                return (float)maxPixel;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        public static void DrawSkeleton(this Canvas canvas, IBodyWrapper body)
        {
            canvas.Children.Clear();
            if (body == null) return;

            // Torso
            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
            canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);

            // Right Arm
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
            canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.ThumbRight]);

            // Left Arm
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
            canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.ThumbLeft]);

            // Right Leg
            canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
            canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
            canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);

            // Left Leg
            canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
            canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
            canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);

            foreach (var joint in body.Joints.Values)
            {
                canvas.DrawPoint(joint);
            }

            // Draw Handstate
            canvas.DrawHand(body.HandRightState, body.Joints[JointType.HandRight]);
            canvas.DrawHand(body.HandLeftState, body.Joints[JointType.HandLeft]);
        }

        public static void DrawPoint(this Canvas canvas, Joint joint)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            var ellipse = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = joint.TrackingState == TrackingState.Tracked ? new SolidColorBrush(Color.FromArgb(255, 68, 192, 68)) : Brushes.Yellow
            };

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = first.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            var thickness = 1;
            var stroke = new SolidColorBrush(Colors.Gray);
            if ((first.TrackingState == TrackingState.Tracked) && (second.TrackingState == TrackingState.Tracked))
            {
                thickness = 8;
                stroke = new SolidColorBrush(Colors.Red);
            }

            var line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = thickness,
                Stroke = stroke
            };

            canvas.Children.Add(line);
        }

        private static void DrawHand(this Panel canvas, HandState handState, Joint joint)
        {
            var brush = new SolidColorBrush(Colors.Transparent);
            switch (handState)
            {
                case HandState.Closed:
                    brush = (SolidColorBrush) HandClosedBrush;
                    break;
                case HandState.Open:
                    brush = (SolidColorBrush) HandOpenBrush;
                    break;
                case HandState.Lasso:
                    brush = (SolidColorBrush) HandLassoBrush;
                    break;
            }

            var ellipse = new Ellipse
            {
                Width = 120,
                Height = 120,
                Fill = brush
            };

            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Save the content of an canvas to a file.
        /// https://stackoverflow.com/questions/5851168/save-canvas-to-bitmap
        /// </summary>
        /// <param name="surface">input canvas</param>
        /// <param name="filename">desired filename</param>
        public static void SaveToFile(this Canvas surface, string filename)
        {
            var size = new Size(surface.ActualWidth, surface.ActualHeight);

            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            var renderBitmap =
                new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (var outStream = new FileStream(filename, FileMode.Create))
            {
                var encoder = new BmpBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }
        }
    }
}
