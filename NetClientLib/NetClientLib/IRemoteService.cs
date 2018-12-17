using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetClientLib
{
    public interface IRemoteService
    {
        IList<CloudPoint> GetCloudpoints();

        string GetSkeletonData();

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
