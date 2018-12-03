using Base.ViewModel;
using KinectLib.Classes;
using KinectLib.Interfaces;
using LightBuzz.Vitruvius;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NetClientLib;
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

        private WriteableBitmap GenerateImageFromPointCloud(IEnumerable<CloudPoint> iCloudPoints, PointCloudVisualization iPointCloudVisualization = PointCloudVisualization.Color)
        {
            //http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/

            const int height = 424;
            const int width = 512;

            var wbitmap = new WriteableBitmap(pixelWidth: width, pixelHeight: height, dpiX: 96, dpiY: 96,
                                              pixelFormat: PixelFormats.Default, palette: null);


            var pixels = new byte[height, width, 4];

            // Clear to black.
            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        pixels[row, col, i] = 0;
                    }
                    pixels[row, col, 3] = 255;
                }
            }

            foreach (var cloudPoint in iCloudPoints)
            {
                if (iPointCloudVisualization == PointCloudVisualization.Color)
                {
                    // Blue
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 0] = (byte)cloudPoint.GetB();
                    // Green
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 1] = (byte)cloudPoint.GetG();
                    // Red
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 2] = (byte)cloudPoint.GetR();
                }
                else
                {
                    // Blue
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 0] = (byte)(cloudPoint.GetZ() % 4096);
                    // Green
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 1] = (byte)(cloudPoint.GetZ() % 4096);
                    // Red
                    pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 2] = (byte)(cloudPoint.GetZ() % 4096);
                }

                // Alpha
                pixels[(int)cloudPoint.GetY(), (int)cloudPoint.GetX(), 3] = 255;
            }

            // Copy the data into a one-dimensional array.
            var pixels1D = new byte[height * width * 4];
            var index = 0;
            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        pixels1D[index++] = pixels[row, col, i];
                    }
                        
                }
            }

            var rect = new Int32Rect(0, 0, width, height);
            const int stride = 4 * width;
            wbitmap.WritePixels(rect, pixels1D, stride, 0);

            return wbitmap;
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
