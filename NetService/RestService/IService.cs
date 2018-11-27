using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace NetService.RestService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet]
        string Data();

        [OperationContract]
        [WebGet]
        string SkeletonData();

        [OperationContract]
        [WebGet]
        string Status();
    }
}