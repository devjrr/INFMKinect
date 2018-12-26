using System.Windows.Media;
using LightBuzz.Vitruvius;

namespace KinectLib.Classes.Impl
{
    public class FrameCaptureWrapper : IFrameCaptureWrapper
    {
        public void SaveImage(ImageSource iSource, string iPath)
        {
            var capture = new FrameCapture();
            capture.Save(iSource, iPath);
        }
    }
}
