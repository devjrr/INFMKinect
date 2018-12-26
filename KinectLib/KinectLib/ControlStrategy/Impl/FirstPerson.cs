using Microsoft.Kinect;
using System.Linq;

namespace KinectLib.ControlStrategy.Impl
{
    public class FirstPerson : IControlStrategy
    {
        public Body GetPerson(Body[] iBodies)
        {
            return iBodies.FirstOrDefault(x => x.IsTracked);
        }
    }
}
