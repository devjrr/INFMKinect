using System;
using System.Collections.Generic;
using System.Text;

namespace NetClientLib
{
    public class RemoteServiceBuilder
    {
        public static IRemoteService GetRemoteService()
        {
            return new RemoteService();
        }
    }
}
