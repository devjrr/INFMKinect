using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectLib.Interfaces;
using Microsoft.Kinect;

namespace KinectLib.ControlStrategy
{
    public class ClosestPerson : IControlStrategy
    {
        public Body GetPerson(Body[] iBodies)
        {
            Body body = null;
            int activeBodyIndex = -1;

            float minZPoint = float.MaxValue;
            for (int i = 0; i < iBodies.Length; i++)
            {
                body = iBodies[i];
                if (body.IsTracked)
                {
                    float zMeters = body.Joints[JointType.SpineBase].Position.Z;
                    if (zMeters < minZPoint)
                    {
                        minZPoint = zMeters;
                        activeBodyIndex = i;
                    }
                }
            }
            if (activeBodyIndex != -1)
            {
                body = iBodies[activeBodyIndex];
            }

            return body;
        }
    }
}
