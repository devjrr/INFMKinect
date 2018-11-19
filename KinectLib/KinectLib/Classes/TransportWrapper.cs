using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectLib.Classes
{
    [Serializable]
    public class TransportWrapper
    {
        public TransportWrapper()
        {
        }

        public BodyWrapper Skeleton { get; set; }

        public List<PointData> PointDatas { get; set; }
    }
}
