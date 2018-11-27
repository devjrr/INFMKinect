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

        public RestService(ISerializer source)
        {
            int port = Properties.Settings.Default.Port;
            this.source = source;
            Uri url = new Uri("http://localhost:" + port);
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
            WriteProgress(framerate.FrameRate + " Responses per Second", 5);

            byte[] arr = source.getData();
            if (arr == null)
            {
                Console.WriteLine("Kinect not available");
            }


            string base64 = Convert.ToBase64String(arr);
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
            return base64;
        }

        public string SkeletonData()
        {
            return source.getSkeletonData();
        }

        protected static void WriteProgress(string s, int x)
        {
            // from https://stackoverflow.com/a/49099413

            int origRow = Console.CursorTop;
            int origCol = Console.CursorLeft;
            // Console.WindowWidth = 10;  // this works. 
            int width = Console.WindowWidth;
            x = x % width;
            try
            {
                Console.SetCursorPosition(x, 5);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
            finally
            {
                try
                {
                    Console.SetCursorPosition(origRow, origCol);
                }
                catch (ArgumentOutOfRangeException e)
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
