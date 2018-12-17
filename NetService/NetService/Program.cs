using KinectLib.Interfaces;
using NetService.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KinectLib.Classes;

namespace NetService
{
    class Program
    {
        static void Main(string[] args)
        {
            IKinectData source = new KinectData();
            Thread.Sleep(500);
            ISerializer serializer = source.IsKinectConnected() ? new SingleFrameTransportData(source) : DemoTransportData.ReadfromLocalFiles();
            INetService service = new RestService.RestService(serializer);
            service.run();
        }
    }
}
