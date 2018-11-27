using System;
using System.Collections.Generic;
using System.Xml;
using NetClientLib;
using RestSharp;

namespace WpfClient.Proxy
{
    public class WebServiceProxy
    {
        #region Fields
        private IRemoteService _restClient;
        #endregion


        #region Constructor

        public WebServiceProxy()
        {
            _restClient = RemoteServiceBuilder.GetRemoteService();
        }

        public String GetSkeleton()
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

        #region Methods

        private String GetData(IRestResponse iResponse)
        {
            var json = iResponse.Content;
            if (string.IsNullOrEmpty(json)) return null;

            var xml = new XmlDocument();
            xml.LoadXml(json);
            if (string.IsNullOrEmpty(xml.InnerText)) return null;

            return xml.InnerText;
        }
        #endregion
    }
}
