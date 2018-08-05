using Base.ViewModel;
using KinectLib.Classes;
using KinectLib.ControlStrategy;
using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WpfClient.Enums;
using WpfClient.Extensions;
using WpfClient.Properties;
using WpfClient.Proxy;
using TabItem = WpfClient.Enums.TabItem;

namespace WpfClient
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private readonly KinectSensor _sensor = KinectSensor.GetDefault();
        private readonly MultiSourceFrameReader _reader;
        private readonly bool _saveJson;

        // Webservice Timer
        private readonly Timer _webServiceTimer;
        private readonly WebServiceProxy _webServiceProxy;

        private Canvas _skeletonCanvas;
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _saveJson = Settings.Default.SaveJson;

            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            _sensor.IsAvailableChanged += KinectSensor_IsAvailableChanged;

            _sensor.Open();

            StatusText = _sensor.IsAvailable ? Resources.RunningStatusText : Resources.NoSensorStatusText;

            // Webservice Timer
            _webServiceTimer = new Timer(60);
            _webServiceProxy = new WebServiceProxy();
            _webServiceTimer.Elapsed += WebServiceTimer_Elapsed;

            _webServiceTimer.Enabled = !_sensor.IsAvailable;
        }
        #endregion

        #region Properties

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (Equals(_statusText, value)) return;
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private ImageSource _streamImageSource;
        public ImageSource StreamImageSource
        {
            get => _streamImageSource;
            set
            {
                if (Equals(_streamImageSource, value)) return;
                _streamImageSource = value;
                OnPropertyChanged("StreamImageSource");
            }
        }

        private ImageSource _skeletonImageSource;
        public ImageSource SkeletonImageSource
        {
            get => _skeletonImageSource;
            set
            {
                if (Equals(_skeletonImageSource, value)) return;
                _skeletonImageSource = value;
                OnPropertyChanged("SkeletonImageSource");
            }
        }

        private ImageSource _pointCloudImageSource;
        public ImageSource PointCloudImageSource
        {
            get => _pointCloudImageSource;
            set
            {
                if (Equals(_pointCloudImageSource, value)) return;
                _pointCloudImageSource = value;
                OnPropertyChanged("PointCloudImageSource");
            }
        }

        private Visualization _streamVisualization = Visualization.Color;
        public Visualization StreamVisualization
        {
            get => _streamVisualization;
            set
            {
                if (_streamVisualization == value) return;
                _streamVisualization = value;
                OnPropertyChanged("StreamVisualization");
            }
        }

        private PointCloudVisualization _pointCloudVisualization;
        public PointCloudVisualization PointCloudVisualization
        {
            get => _pointCloudVisualization;
            set
            {
                if (_pointCloudVisualization == value) return;
                _pointCloudVisualization = value;
                OnPropertyChanged("PointCloudVisualization");
            }
        }

        private TabItem _selectedTabItem = TabItem.Stream;
        public TabItem SelectedTabItem
        {
            get => _selectedTabItem;
            set
            {
                if (_selectedTabItem == value) return;
                _selectedTabItem = value;
                OnPropertyChanged("SelectedTabItem");
            }
        }
        #endregion

        #region Methods

        public void Init(Canvas iCanvas)
        {
            _skeletonCanvas = iCanvas;
        }

        public void Close()
        {
            _reader?.Dispose();

            _sensor?.Close();
        }

        private void SaveJson(String iFileNamePrefix, String iJson, Object iObject)
        {
            if (_saveJson)
            {
                Task.Run(() =>
                {
                    var tick = DateTime.Now.Ticks;
                    if (!File.Exists(iFileNamePrefix + tick + ".json"))
                    {
                        var json = iJson;
                        if (string.IsNullOrEmpty(json))
                        {
                            json = JsonConvert.SerializeObject(iObject);
                        }

                        try
                        {
                            File.WriteAllText(iFileNamePrefix + tick + ".json", json);
                        }
                        catch (Exception e)
                        {
                        }
                        
                    }
                });
            }
        }

        #endregion

        #region Events
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            #region Handle Stream
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (StreamVisualization == Visualization.Color)
                    {
                        StreamImageSource = frame.ToBitmap();
                    }
                }
            }
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (StreamVisualization == Visualization.Depth)
                    {
                        StreamImageSource = frame.ToBitmap();
                    }
                }
            }
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (StreamVisualization == Visualization.Infrared)
                    {
                        StreamImageSource = frame.ToBitmap();
                    }
                }
            }
            #endregion

            #region Handle Skeleton

            if (SelectedTabItem == TabItem.Skeleton)
            {
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

                            // Test Json Export
                            SaveJson("skeleton", null, bodyWrapper);
                            //var bodyFromJson = JsonConvert.DeserializeObject<BodyWrapper>(json);

                            if (_skeletonCanvas != null)
                                _skeletonCanvas.DrawSkeleton(bodyWrapper);
                        }
                    }
                }
            }
            #endregion

            #region Handle Point Cloud

            if (SelectedTabItem == TabItem.PointCloud)
            {
                if (PointCloudVisualization == PointCloudVisualization.Color)
                {
                    using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
                    using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
                    using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
                    {
                        if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                        {
                            var colorPointCloud = new ColorPointCloud(colorFrame, depthFrame, bodyIndexFrame, _sensor.CoordinateMapper);

                            // Test Json Export
                            SaveJson("colorPointCloud", null, colorPointCloud);
                            //var colorPointCloudFromJson = JsonConvert.DeserializeObject<ColorPointCloud>(json);

                            PointCloudImageSource = colorPointCloud.GenerateImage();
                        }
                    }
                }
                else
                {
                    using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
                    using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
                    {
                        if (depthFrame != null && bodyIndexFrame != null)
                        {
                            var highlightedPointCloud = new HighlightedPointCloud(depthFrame, bodyIndexFrame);

                            // Test Json Export
                            SaveJson("highlightedPointCloud", null, highlightedPointCloud);
                            //var highlightedPointCloudFromJson = JsonConvert.DeserializeObject<HighlightedPointCloud>(json);

                            PointCloudImageSource = highlightedPointCloud.GenerateImage();
                        }
                    }
                }
            }


            #endregion

        }

        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            StatusText = _sensor.IsAvailable ? Resources.RunningStatusText : Resources.SensorNotAvailableStatusText;
            _webServiceTimer.Enabled = !_sensor.IsAvailable;
        }

        private void WebServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string json;

            switch (SelectedTabItem)
            {
                case TabItem.Stream:
                    if (StreamVisualization == Visualization.Color)
                    {
                        // Get Color Stream
                        // Output
                    }
                    else if (StreamVisualization == Visualization.Depth)
                    {
                        // Get Depth Stream
                        // Output
                    }
                    else if (StreamVisualization == Visualization.Infrared)
                    {
                        // Get Infrared Stream
                        // Output
                    }
                    break;
                case TabItem.Skeleton:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Get Skeleton Data
                        json = _webServiceProxy.GetSkeleton();
                        if (string.IsNullOrEmpty(json)) return;

                        // Save Json
                        SaveJson("skeleton", json, null);

                        // Output
                        var bodyFromJson = JsonConvert.DeserializeObject<BodyWrapper>(json);

                        if (_skeletonCanvas != null)
                            _skeletonCanvas.DrawSkeleton(bodyFromJson);
                        
                    }), DispatcherPriority.Background);
                    
                    break;
                case TabItem.PointCloud:
                    if (PointCloudVisualization == PointCloudVisualization.Color)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // Get Color PointCloud
                            json = _webServiceProxy.GetColorPointCloud();
                            if (string.IsNullOrEmpty(json)) return;

                            // Save Json
                            SaveJson("colorPointCloud", json, null);

                            // Output
                            var colorPointCloudFromJson = JsonConvert.DeserializeObject<ColorPointCloud>(json);
                        
                            PointCloudImageSource = colorPointCloudFromJson.GenerateImage();
                        }), DispatcherPriority.Background);
                        
                    }
                    else if (PointCloudVisualization == PointCloudVisualization.Highlighted)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // Get Highlighted PointCloud
                            json = _webServiceProxy.GetHighlightedPointCloud();
                            if (string.IsNullOrEmpty(json)) return;

                            // Save Json
                            SaveJson("highlightedPointCloud", json, null);

                            // Output
                            var highlightedPointCloudFromJson = JsonConvert.DeserializeObject<HighlightedPointCloud>(json);
                        
                            PointCloudImageSource = highlightedPointCloudFromJson.GenerateImage();
                        }), DispatcherPriority.Background);
                        
                    }
                    break;
            }
        }
        #endregion
    }


}
