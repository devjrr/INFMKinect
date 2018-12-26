using System.Collections.Generic;
using Microsoft.Kinect;

namespace KinectLib.Classes
{
    public interface IBodyWrapper
    {
        bool IsTracked { get; }
        ulong TrackingId { get; }
        Dictionary<JointType, Joint> Joints { get; }
        Dictionary<JointType, JointOrientation> JointOrientations { get; }
        HandState HandLeftState { get; }
        HandState HandRightState { get; }
    }
}
