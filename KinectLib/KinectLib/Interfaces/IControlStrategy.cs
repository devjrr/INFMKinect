using Microsoft.Kinect;

namespace KinectLib.Interfaces
{
    // http://kinect.github.io/tutorial/lab14/index.html
    public interface IControlStrategy
    {
        Body GetPerson(Body[] iBodies);
    }
}
