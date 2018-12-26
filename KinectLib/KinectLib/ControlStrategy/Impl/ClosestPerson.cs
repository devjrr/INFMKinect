using Microsoft.Kinect;

namespace KinectLib.ControlStrategy.Impl
{
    public class ClosestPerson : IControlStrategy
    {
        public Body GetPerson(Body[] iBodies)
        {
            Body body = null;
            var activeBodyIndex = -1;

            var minZPoint = float.MaxValue;
            for (var i = 0; i < iBodies.Length; i++)
            {
                body = iBodies[i];
                if (!body.IsTracked) continue;
                var zMeters = body.Joints[JointType.SpineBase].Position.Z;

                if (!(zMeters < minZPoint)) continue;
                minZPoint = zMeters;
                activeBodyIndex = i;
            }
            if (activeBodyIndex != -1)
            {
                body = iBodies[activeBodyIndex];
            }

            return body;
        }
    }
}
