using LightBuzz.Vitruvius;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfClient.Enums;
using WpfClient.Extensions;

namespace WpfClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields
        private MainViewModel _vm;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Events
        #region Window Events
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as MainViewModel;
            _vm.Init(SkeletonCanvas);
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((System.Windows.Controls.TabItem)e.AddedItems[0]).Name)
            {
                case "StreamTabItem":
                    _vm.SelectedTabItem = Enums.TabItem.Stream;
                    break;
                case "SkeletonTabItem":
                    _vm.SelectedTabItem = Enums.TabItem.Skeleton;
                    break;
                case "PointCloudTabItem":
                    _vm.SelectedTabItem = Enums.TabItem.PointCloud;
                    break;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _vm.Close();
        }
        #endregion

        #region Click Events
        private void ColorStreamButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.StreamVisualization = Visualization.Color;
            DepthStreamButton.IsChecked = false;
            InfraredStreamButton.IsChecked = false;
        }

        private void DepthStreamButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.StreamVisualization = Visualization.Depth;
            ColorStreamButton.IsChecked = false;
            InfraredStreamButton.IsChecked = false;
        }

        private void InfraredStreamButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.StreamVisualization = Visualization.Infrared;
            ColorStreamButton.IsChecked = false;
            DepthStreamButton.IsChecked = false;
        }


        private void PointCloudColorButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.PointCloudVisualization = PointCloudVisualization.Color;
            PointCloudHighlightedButton.IsChecked = false;
        }

        private void PointCloudHighlightedButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.PointCloudVisualization = PointCloudVisualization.Highlighted;
            PointCloudColorButton.IsChecked = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var capture = new FrameCapture();

            switch (_vm.SelectedTabItem)
            {
                case Enums.TabItem.Stream:
                    capture.Save(_vm.StreamImageSource, "stream_" + DateTime.Now.Ticks + ".jpg");
                    break;
                case Enums.TabItem.Skeleton:
                    capture.Save(((DrawingImage)_vm.SkeletonImageSource).ToBitmapSource(), "skeleton_" + DateTime.Now.Ticks + ".jpg");
                    break;
                case Enums.TabItem.PointCloud:
                    capture.Save(_vm.PointCloudImageSource, "pointCloud_" + DateTime.Now.Ticks + ".jpg");
                    break;
            }
        }
        #endregion

        #endregion
    }
}
