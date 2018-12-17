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
        WebServiceHost serviceHost;
        ISerializer source;
        Framerate framerate = new Framerate();
        private long _totalRequests = 0;

        public RestService(ISerializer source)
        {
            var port = Properties.Settings.Default.PORT;
            this.source = source;
            var url = new Uri("http://localhost:" + port);
            serviceHost = new WebServiceHost(this as IService, url);

            Console.WriteLine(url);
            foreach (MemberInfo info in typeof(IService).GetMembers())
            {
                if (info.MemberType == MemberTypes.Method)
                {
                    Console.WriteLine(info.Name);
                }
            }
        }

        public string Data()
        {
            framerate.MeasureHere();
            WriteProgress(framerate.FrameRate + " Responses per Second      ", 5, 5);
            WriteProgress("Total requests: " + ++_totalRequests, 5, 6);

            var arr = source.getData();
            if (arr == null)
            {
                Console.WriteLine("Kinect not available");
            }


            var base64 = Convert.ToBase64String(arr);
            return base64;
        }

        public string SkeletonData()
        {
            return source.getSkeletonData();
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
            serviceHost.Open();
            Console.ReadLine();
            serviceHost.Close();
        }

        public string Status()
        {
            byte[] data = null;
            try
            {
                data = source.getData();
            }
            catch
            {
                return "offline";
            }

            if (data == null)
            {
                return "offline";
            }

            return "online";

        }
    }
}
