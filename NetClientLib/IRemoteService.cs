using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetClientLib
{
    public interface IRemoteService
    {
        IList<CloudPoint> GetCloudpoints();

        bool isServerOnline();

        bool isKinectOnline();

        /***
         * Returns the current network upload in KB/s (1KB = 1024 Bytes)
         */
        float GetCurrentUpload();

        /***
       * Returns the current network download in KB/s (1KB = 1024 Bytes)
       */
        float GetCurrentDownload();
    }
}
