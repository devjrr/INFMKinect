using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace KinectServer.Interfaces
{
    [ServiceContract(Name = "KinectRestService")]
    interface IKinectRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.Routing.GetKinectData + "/skeleton", BodyStyle = WebMessageBodyStyle.Bare)]
        String GetSkeleton();

        [OperationContract]
        [WebGet(UriTemplate = Routing.Routing.GetKinectData + "/colorpointcloud", BodyStyle = WebMessageBodyStyle.Bare)]
        String GetColorPointCloud();

        [OperationContract]
        [WebGet(UriTemplate = Routing.Routing.GetKinectData + "/highlightedpointcloud", BodyStyle = WebMessageBodyStyle.Bare)]
        String GetHighlightedPointCloud();

        [OperationContract]
        [WebGet(UriTemplate = Routing.Routing.GetKinectData + "/transportdata", BodyStyle = WebMessageBodyStyle.Bare)]
        String GetTransportData();

    }
}
