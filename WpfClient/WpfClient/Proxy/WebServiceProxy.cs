using System.Windows.Media.Imaging;
using NetClientLib;
using WpfClient.Extensions;

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

        public BitmapSource GetColorPointCloud()
        {
            var response = _restClient.GetCloudpoints();

            var bitmap = response.GenerateColorBitmap();

            return bitmap;
        }

        public BitmapSource GetDepthPointCloud()
        {
            var response = _restClient.GetCloudpoints();

            var bitmap = response.GenerateDepthBitmap();

            return bitmap;
        }
        #endregion
    }
}
