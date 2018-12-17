using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NetClientLib
{
    public class RemoteServiceBuilder
    {
        private const string localhost = "http://localhost:56789/";

        // Must be matching the dimensions the server supplies
        private const int height = 212;
        private const int width = 256;

        private static readonly Deserializer deserializer = new Deserializer();

#if !UNITY

        public static IRemoteService GetRemoteService(string url)
        {
            return new RemoteService(deserializer, url, height, width);
        }

        public static IRemoteService GetRemoteService()
        {
            return new RemoteService(deserializer, localhost, height, width);
        }
#endif

#if UNITY

        public static IRemoteService GetRemoteService(MonoBehaviour monoBehaviour)
        {
            return new UnityRemoteService(monoBehaviour, deserializer, localhost, height, width);
        }

        public static IRemoteService GetRemoteService(MonoBehaviour monoBehaviour, string url)
        {
            return new UnityRemoteService(monoBehaviour, deserializer, url, height, width);
        }
#endif
    }
}
