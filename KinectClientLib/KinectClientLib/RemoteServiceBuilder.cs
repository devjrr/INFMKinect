using System;
using System.Collections.Generic;
using System.Text;

namespace NetClientLib
{
    public class RemoteServiceBuilder
    {
        private const string localhost = "http://localhost:56789/";

        // Must be matching the dimensions the server supplies
        private const int height = 212;
        private const int width = 256;


        public static IRemoteService GetRemoteService(string url)
        {
            return new RemoteService(url, height, width);
        }

        public static IRemoteService GetRemoteService()
        {
            return new RemoteService(localhost, height, width);
        }
    }
}
