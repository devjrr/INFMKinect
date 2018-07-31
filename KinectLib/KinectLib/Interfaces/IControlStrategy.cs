using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectLib.Interfaces
{
    // http://kinect.github.io/tutorial/lab14/index.html
    public interface IControlStrategy
    {
        Body GetPerson(Body[] iBodies);
    }
}
