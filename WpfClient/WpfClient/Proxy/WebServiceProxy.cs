using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RestSharp;

namespace WpfClient.Proxy
{
    public class WebServiceProxy
    {
        #region Fields
        private RestClient _restClient;
        #endregion


        #region Constructor

        public WebServiceProxy()
        {
            _restClient = new RestClient(Properties.Settings.Default.ApiKey + "GetKinectData");
        }

        public String GetSkeleton()
        {
            var response = _restClient.Execute(new RestRequest("/skeleton"));

            return GetData(response);
        }

        public String GetColorPointCloud()
        {
            var response = _restClient.Execute(new RestRequest("/colorpointcloud"));

            return GetData(response);
        }

        public String GetHighlightedPointCloud()
        {
            var response = _restClient.Execute(new RestRequest("/highlightedpointcloud"));

            return GetData(response);
        }

        public String GetTransportData()
        {
            var response = _restClient.Execute(new RestRequest("/transportdata"));

            return GetData(response);
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
