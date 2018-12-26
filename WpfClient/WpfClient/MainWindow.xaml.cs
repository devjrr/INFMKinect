using System;
using System.Windows;
using System.Windows.Controls;
using KinectLib.Classes.Impl;
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
            if(_vm == null) return;

            _vm.Init(SkeletonCanvas);
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((System.Windows.Controls.TabItem)e.AddedItems[0]).Name)
            {
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

        private void PointCloudColorButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.PointCloudVisualization = PointCloudVisualization.Color;
            PointCloudDepthButton.IsChecked = false;
        }

        private void PointCloudDepthButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.PointCloudVisualization = PointCloudVisualization.Depth;
            PointCloudColorButton.IsChecked = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_vm.SelectedTabItem)
            {
                case Enums.TabItem.Skeleton:
                    SkeletonCanvas.SaveToFile(filename: "skeleton_" + DateTime.Now.Ticks + ".jpg");
                    break;
                case Enums.TabItem.PointCloud:
                    new FrameCaptureWrapper().SaveImage(_vm.PointCloudImageSource, "pointCloud_" + DateTime.Now.Ticks + ".jpg");
                    break;
            }
        }

        #endregion

        #endregion
    }
}
