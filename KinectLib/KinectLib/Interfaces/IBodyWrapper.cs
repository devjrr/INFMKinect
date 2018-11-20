using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectLib.Interfaces
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
