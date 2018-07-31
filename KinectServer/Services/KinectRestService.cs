using KinectLib.Classes;
using KinectLib.ControlStrategy;
using KinectLib.Interfaces;
using KinectServer.Interfaces;
using Microsoft.Kinect;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Newtonsoft.Json;

namespace KinectServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class KinectRestService : IKinectRestService
    {
        private const string SERVICE_NAME = "KinectService";

        private readonly KinectSensor _sensor = KinectSensor.GetDefault();
        private MultiSourceFrameReader _reader;
        private String _skeletonJson;
        private String _highlightedPointCloudJson;
        private String _colorPointCloudJson;

        #region Properties
        private IControlStrategy _controlStrategy = new ClosestPerson();
        public IControlStrategy ControlStrategy
        {
            get => _controlStrategy;
            set
            {
                if (_controlStrategy == value) return;
                if (value == null) return;

                _controlStrategy = value;
            }
        }
        #endregion

        #region Constructor
        public KinectRestService()
        {
            InitReader();
        }
        #endregion

        #region Methods
        public string GetServiceName()
        {
            return SERVICE_NAME;
        }

        private void InitReader()
        {
            if (_sensor == null) return;

            _sensor.Open();
            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

        }

        public string GetSkeleton()
        {
            return _skeletonJson;
        }

        public string GetHighlightedPointCloud()
        {
            return _highlightedPointCloudJson;
        }

        public string GetColorPointCloud()
        {
            return _colorPointCloudJson;
        }

        public void Shutdown()
        {
            _reader.Dispose();
            _sensor.Close();
        }
        #endregion

        #region Events
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
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

                        // Json Export
                        _skeletonJson = JsonConvert.SerializeObject(bodyWrapper);
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
                    // Json Export
                    _colorPointCloudJson = JsonConvert.SerializeObject(colorPointCloud);
                }

            }


            using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
            using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
            {
                if (depthFrame != null && bodyIndexFrame != null)
                {
                    var highlightedPointCloud = new HighlightedPointCloud(depthFrame, bodyIndexFrame);
                    
                    // Json Export
                    _highlightedPointCloudJson = JsonConvert.SerializeObject(highlightedPointCloud);
                }
            }

            #endregion
        }
        #endregion

    }
}
