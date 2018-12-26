using System.Windows.Media;

namespace KinectLib.Classes
{
    public interface IFrameCaptureWrapper
    {
        void SaveImage(ImageSource iSource, string iPath);
    }
}
