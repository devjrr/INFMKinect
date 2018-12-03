using System.Collections.Generic;
using NetClientLib;

namespace WpfClient.Proxy
{
    public class WebServiceProxy
    {
        #region Fields
        private readonly IRemoteService _restClient;
        #endregion


        #region Constructor

        public WebServiceProxy()
        {
            _restClient = RemoteServiceBuilder.GetRemoteService();
        }

        public string GetSkeleton()
        {
            var response = _restClient.GetSkeletonData();
            return response;
        }

        public IList<CloudPoint> GetColorPointCloud()
        {
            var response = _restClient.GetCloudpoints();

            return response;
        }

        public IList<CloudPoint> GetDepthPointCloud()
        {
            var response = _restClient.GetCloudpoints();

            return response;
        }
        #endregion
    }
}
