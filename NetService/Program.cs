using KinectLib.Interfaces;
using KinectServer.Services;
using NetService.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetService
{
    class Program
    {
        static void Main(string[] args)
        {
            IKinectData source = new KinectRestService();
            ISerializer serializer = new SingleFrameTransportData(source);
            INetService service = new RestService.RestService(serializer);
            service.run();
        }
    }
}
