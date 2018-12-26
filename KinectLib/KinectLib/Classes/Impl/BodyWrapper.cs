using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;

namespace KinectLib.Classes.Impl
{
    [Serializable]
    public class BodyWrapper : IBodyWrapper
    {
        public BodyWrapper()
        {
            
        }

        public BodyWrapper(Body iBody)
        {
            IsTracked = iBody.IsTracked;
            TrackingId = iBody.TrackingId;
            Joints = iBody.Joints.ToDictionary(pair => pair.Key, pair => pair.Value);
            JointOrientations = iBody.JointOrientations.ToDictionary(pair => pair.Key, pair => pair.Value);
            HandLeftState = iBody.HandLeftState;
            HandRightState = iBody.HandRightState;
        }

        public bool IsTracked { get; set; }
        public ulong TrackingId { get; set; }
        public Dictionary<JointType, Joint> Joints { get; set; }
        public Dictionary<JointType, JointOrientation> JointOrientations { get; set; }
        public HandState HandLeftState { get; set; }
        public HandState HandRightState { get; set; }
    }
}
