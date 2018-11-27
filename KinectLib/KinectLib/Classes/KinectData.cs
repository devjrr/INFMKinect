using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectLib.ControlStrategy;
using KinectLib.Interfaces;
using Microsoft.Kinect;

namespace KinectLib.Classes
{
    public class KinectData : IKinectData
    {
        #region Fields
        private readonly KinectSensor _sensor = KinectSensor.GetDefault();
        private MultiSourceFrameReader _reader;
        private bool _readFrame;
        #endregion

        #region Properties
        public IBodyWrapper CurrentSkeleton { get; set; }

        public ColorPointCloud CurrentColorPointCloud { get; set; }
        #endregion

        #region Constructor
        public KinectData()
        {
            InitReader();
        }
        #endregion

        #region Methods
        private void InitReader()
        {
            if (_sensor == null) return;

            _sensor.Open();
            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

        }

        #endregion

        #region Events
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (_readFrame) return;

            _readFrame = true;

            var reference = e.FrameReference.AcquireFrame();
            if (reference == null) return;

            #region Handle Skeleton
            using (var bodyFrame = reference.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    var bodies = new Body[bodyFrame.BodyFrameSource.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodies);

                    var body = new ClosestPerson().GetPerson(bodies);
                    if (body != null)
                    {
                        var bodyWrapper = new BodyWrapper(body);
                        CurrentSkeleton = bodyWrapper;
                    }
                }
            }
            #endregion

            #region Handle Point Cloud

            using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
            using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
            using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
            {

                if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                {
                    var colorPointCloud = new ColorPointCloud(colorFrame, depthFrame, bodyIndexFrame, _sensor.CoordinateMapper);
                    CurrentColorPointCloud = colorPointCloud;
                }

            }

            #endregion

            _readFrame = false;
        }
        #endregion


        #region IKinectData
        public bool[] GetBodyData()
        {
            return CurrentColorPointCloud.BodyData;
        }

        public byte[] GetColorData()
        {
            return CurrentColorPointCloud.ColorData;
        }

        public ushort[] GetDepthData()
        {
            return CurrentColorPointCloud.DepthData;
        }

        IBodyWrapper IKinectData.GetSkeleton()
        {
            return CurrentSkeleton;
        }

        public void Shutdown()
        {
            _reader.Dispose();
            _sensor.Close();
        }

        public bool IsKinectConnected()
        {
            return _sensor.IsAvailable;
        }
        #endregion
    }
}
