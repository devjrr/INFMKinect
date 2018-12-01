using Base.ViewModel;
using KinectLib.Classes;
using KinectLib.Interfaces;
using LightBuzz.Vitruvius;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private readonly bool _saveJson;

        private readonly IKinectData _kinectData;
        private bool _dataFromWebService;

        // Webservice Timer
        private readonly WebServiceProxy _webServiceProxy;

        private Canvas _skeletonCanvas;
        private Canvas _pointCloudCanvas;
        private Canvas _depthPointCloudCanvas;
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _saveJson = Settings.Default.SaveJson;

            _kinectData = new KinectData();

            CheckKinectConnected();

            // Webservice Timer
            var webServiceTimer = new Timer(60);
            _webServiceProxy = new WebServiceProxy();
            webServiceTimer.Elapsed += WebServiceTimer_Elapsed;
            webServiceTimer.Enabled = true;
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

        private TabItem _selectedTabItem = TabItem.Skeleton;
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

        public void Init(Canvas iCanvas, Canvas iCanvas2, Canvas iCanvas3)
        {
            _skeletonCanvas = iCanvas;
            _pointCloudCanvas = iCanvas2;
            _depthPointCloudCanvas = iCanvas3;
        }

        private void CheckKinectConnected()
        {
            StatusText = _kinectData.IsKinectConnected() ? Resources.RunningStatusText : Resources.SensorNotAvailableStatusText;
            _dataFromWebService = !_kinectData.IsKinectConnected();
        }

        public void Close()
        {
            _kinectData.Shutdown();
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
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                });
            }
        }

        #endregion

        #region Events
        private void WebServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Check Kinect Status
            CheckKinectConnected();

            string json = null;
            IBodyWrapper body = null;

            switch (SelectedTabItem)
            {
                case TabItem.Skeleton:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        if (_dataFromWebService)
                        {
                            // Get Skeleton Data
                            json = _webServiceProxy.GetSkeleton();
                            if (string.IsNullOrEmpty(json)) return;
                            body = JsonConvert.DeserializeObject<BodyWrapper>(json);
                        }
                        else
                        {
                            body = _kinectData.GetSkeleton();
                        }


                        // Save Json
                        SaveJson("skeleton", null, body);

                        _skeletonCanvas?.DrawSkeleton(body);


                    }), DispatcherPriority.Background);

                    break;
                case TabItem.PointCloud:
                    if (PointCloudVisualization == PointCloudVisualization.Color)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // Get Color PointCloud
                            var pointCloud = _webServiceProxy.GetColorPointCloud();
                            if (pointCloud == null) return;

                            var width = 256;
                            var height = 212;
                            var pixelFormat = PixelFormats.Rgb24;
                            var bytesPerPixel = 3;
                            var stride = bytesPerPixel * width;

                            byte[] buffer = new byte[width * height * bytesPerPixel];


                            foreach (var p in pointCloud)
                            {
                                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel] = (byte)p.GetR();
                                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel + 1] = (byte)p.GetG();
                                buffer[stride * (int)p.GetY() + (int)p.GetX() * bytesPerPixel + 2] = (byte)p.GetB();
                            }

                            var bitmap = BitmapSource.Create(width, height, Constants.DPI, Constants.DPI, pixelFormat, null, buffer, stride);


                            PointCloudImageSource = bitmap;


                        }), DispatcherPriority.Background);

                    }
                    else if (PointCloudVisualization == PointCloudVisualization.Depth)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // Get Depth PointCloud
                            var pointCloud = _webServiceProxy.GetDepthPointCloud();
                            if (pointCloud == null) return;

                            var width = 256;
                            var height = 212;
                            var pixelFormat = PixelFormats.Gray16;
                            var bytesPerPixel = 2;
                            var stride = bytesPerPixel * width;

                            ushort[] buffer = new ushort[width * height];

                            foreach (var p in pointCloud)
                            {
                                buffer[width * (int)p.GetY() + (int)p.GetX()] = (ushort)p.GetZ();
                            }

                            var bitmap = BitmapSource.Create(width, height, Constants.DPI, Constants.DPI, pixelFormat, null, buffer, stride);

                            PointCloudImageSource = bitmap;

                        }), DispatcherPriority.Background);

                    }
                    break;
            }
        }
        #endregion
    }


}
