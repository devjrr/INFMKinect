using System;
using System.ServiceModel.Web;

namespace KinectServer
{
    class Start
    {
        static void Main(string[] args)
        {
            var settings = Properties.Settings.Default;
            var port = settings.Port;

            var service = new Services.KinectRestService();
            var apiKey = "http://localhost:" + port + "/" + service.GetServiceName();
            var serviceHost = new WebServiceHost(service, new Uri(apiKey));
            serviceHost.Open();
            Console.WriteLine(apiKey + "/GetKinectData");
            while (true)
            {
                var text = Console.ReadLine();

                if (text.ToUpper() == "EXIT") break;
            }
            service.Shutdown();
            serviceHost.Close();
        }
    }
}
