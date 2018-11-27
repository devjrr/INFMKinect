using KinectLib.Interfaces;
using NetService.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectLib.Classes;

namespace NetService
{
    class Program
    {
        static void Main(string[] args)
        {
            IKinectData source = new KinectData();
            ISerializer serializer = new SingleFrameTransportData(source);
            INetService service = new RestService.RestService(serializer);
            service.run();
        }
    }
}
