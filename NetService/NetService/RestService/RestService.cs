using System;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Reflection;

namespace NetService.RestService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                 ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    class RestService : INetService, IService
    {
        #region Fields

        private const string KinectNotAvailable = "Kinect not available";

        private readonly WebServiceHost _serviceHost;
        private readonly ISerializer _source;
        private readonly Framerate _framerate = new Framerate();
        private long _totalRequests;
        #endregion

        #region Constructor
        public RestService(ISerializer iSource)
        {
            var port = Properties.Settings.Default.PORT;
            _source = iSource;
            var url = new Uri("http://localhost:" + port);
            _serviceHost = new WebServiceHost(this, url);

            Console.WriteLine(url);
            foreach (var info in typeof(IService).GetMembers())
            {
                if (info.MemberType == MemberTypes.Method)
                {
                    Console.WriteLine(info.Name);
                }
            }
        }
        #endregion

        #region Methods
        public string Data()
        {
            PrintProgress();

            var arr = _source.getData();
            if (arr == null)
            {
                Console.WriteLine(KinectNotAvailable);
                return string.Empty;
            }


            var base64 = Convert.ToBase64String(arr);
            return base64;
        }

        public string SkeletonData()
        {
            PrintProgress();

            var data = _source.getSkeletonData();
            if (!string.IsNullOrEmpty(data)) return data;

            Console.WriteLine(KinectNotAvailable);
            return string.Empty;

        }

        private void PrintProgress()
        {
            _framerate.MeasureHere();
            WriteProgress(_framerate.FrameRate + " Responses per Second      ", 5, 5);
            WriteProgress("Total requests: " + ++_totalRequests, 5, 6);
        }

        protected static void WriteProgress(string iString, int x, int iLineNumber)
        {
            // from https://stackoverflow.com/a/49099413

            var origRow = Console.CursorTop;
            var origCol = Console.CursorLeft;
            // Console.WindowWidth = 10;  // this works. 
            var width = Console.WindowWidth;
            x = x % width;
            try
            {
                Console.SetCursorPosition(x, iLineNumber);
                Console.Write(iString);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            finally
            {
                try
                {
                    Console.SetCursorPosition(origRow, origCol);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        public void run()
        {
            _serviceHost.Open();
            Console.ReadLine();
            _serviceHost.Close();
        }

        public void Terminate()
        {
            _serviceHost.Close();
        }

        public string Status()
        {
            byte[] data;
            try
            {
                data = _source.getData();
            }
            catch
            {
                return "offline";
            }

            return data == null ? "offline" : "online";
        }
        #endregion
    }
}
