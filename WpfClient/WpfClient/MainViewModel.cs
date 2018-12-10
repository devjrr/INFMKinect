using Base.ViewModel;
using KinectLib.Classes;
using KinectLib.Interfaces;
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

        private readonly IKinectData _kinectData = new KinectData();
        private bool _dataFromWebService;

        // Webservice Timer
        private readonly WebServiceProxy _webServiceProxy;

        private Canvas _skeletonCanvas;
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _saveJson = Settings.Default.SaveJson;

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

        public void Init(Canvas iCanvas)
        {
            _skeletonCanvas = iCanvas;
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

        private void SaveJson(string iFileNamePrefix, string iJson, object iObject)
        {
            if (_saveJson)
            {
                Task.Run(() =>
                {
                    var tick = DateTime.Now.Ticks;
                    if (File.Exists(iFileNamePrefix + tick + ".json")) return;

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
                });
            }
        }

        #endregion

        #region Events
        private void WebServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Check Kinect Status
            CheckKinectConnected();

            string json;
            IBodyWrapper body;

            switch (SelectedTabItem)
            {
                case TabItem.Skeleton:
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
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
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        BitmapSource pointCloud = null;
                        if (_dataFromWebService)
                        {
                            if (PointCloudVisualization == PointCloudVisualization.Color)
                            {
                                // Get Color PointCloud
                                pointCloud = _webServiceProxy.GetColorPointCloud();
                            }
                            else if (PointCloudVisualization == PointCloudVisualization.Depth)
                            {
                                // Get Depth PointCloud
                                pointCloud = _webServiceProxy.GetDepthPointCloud();
                            }
                        }
                        else
                        {
                            if (PointCloudVisualization == PointCloudVisualization.Color)
                            {
                                // Get Color PointCloud
                                pointCloud = _kinectData.GetColorData().GenerateColorBitmap(_kinectData.GetBodyData());
                            }
                            else if (PointCloudVisualization == PointCloudVisualization.Depth)
                            {
                                // Get Depth PointCloud
                                pointCloud = _kinectData.GetDepthData().GenerateDepthBitmap(_kinectData.GetBodyData());
                            }
                        }

                        if (pointCloud == null) return;

                        PointCloudImageSource = pointCloud;

                    }), DispatcherPriority.Background);
                    break;
            }
        }
        #endregion
    }
}
