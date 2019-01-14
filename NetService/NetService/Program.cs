using NetService.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using KinectLib.Classes;
using KinectLib.Classes.Impl;
using Timer = System.Timers.Timer;

namespace NetService
{
    class Program
    {
        private static bool _connected;
        private static IKinectData _source;
        private static INetService _service;

        static void Main(string[] args)
        {
            var presentationMode = Properties.Settings.Default.PRESENTATION_MODE;

            if (presentationMode)
            {
                _source = new KinectData();
                _service = new RestService.RestService(new SingleFrameTransportData(_source));
                _service.run();
                return;
            }

            var timer = new Timer
            {
                Interval = 500,
                Enabled = true,
                AutoReset = true
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            _source = new KinectData();
            _connected = _source.IsKinectConnected();

            var serializer = _connected ? new SingleFrameTransportData(_source) : DemoTransportData.ReadfromLocalFiles();

            _service = new RestService.RestService(serializer);
            _service.run();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(_connected == _source.IsKinectConnected()) return;
            
            _service.Terminate();

            Console.Clear();
            _connected = _source.IsKinectConnected();

            var serializer = _connected ? new SingleFrameTransportData(_source) : DemoTransportData.ReadfromLocalFiles();
            
            _service = new RestService.RestService(serializer);
        }
    }
}
