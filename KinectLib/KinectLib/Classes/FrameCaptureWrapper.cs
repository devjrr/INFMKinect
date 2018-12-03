using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LightBuzz.Vitruvius;

namespace KinectLib.Classes
{
    public static class FrameCaptureWrapper
    {
        public static void SaveImage(ImageSource iSource, string iPath)
        {
            var capture = new FrameCapture();
            capture.Save(iSource, iPath);
        }
    }
}
