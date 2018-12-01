using NetClientLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TestClient
{
    public partial class FormMain : Form
    {
        private System.Timers.Timer timer = new System.Timers.Timer();

        private IRemoteService service = RemoteServiceBuilder.GetRemoteService();


        Framerate targetedFramerate = new Framerate();
        Framerate actualFramerate = new Framerate();

        DirectBitmap bmp = new DirectBitmap(1024, 424);
        bool isConnected = false;
        bool isServerOnline = false;

        public FormMain()
        {
            InitializeComponent();
            Text = "offline";

            timer.Interval = 1000 / 120;
            timer.Elapsed += OnTimerTick;
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            targetedFramerate.MeasureHere();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            actualFramerate.MeasureHere();

            if (isServerOnline)
            {

                string connected = isConnected ? "Kinect ready" : "Kinect unavailable";
                Text = "Server online, " + connected + " " + actualFramerate.FrameRate + "   fps, Target: " + targetedFramerate.FrameRate + " fps,   ↑" + service.GetCurrentUpload() + " KB/s   ↓" + service.GetCurrentDownload() + " KB/s"; // Window Title Text
                if (isConnected)
                {
                    Graphics g = Graphics.FromImage(bmp.Bitmap);
                    g.Clear(Color.Transparent);

                    IList<CloudPoint> cloudpoints = service.GetCloudpoints();

                    if (cloudpoints == null)
                    {
                        g.Clear(Color.Magenta);
                        Text = "Internal server error";
                    }
                    else
                    {
                        foreach (CloudPoint p in cloudpoints)
                        {
                            // Color
                            bmp.SetPixel((int)p.GetX(), (int)p.GetY(), Color.FromArgb((int)p.GetR(), (int)p.GetG(), (int)p.GetB()));

                            // Depth
                            bmp.SetPixel((int)p.GetX() + 512, (int)p.GetY(), Color.FromArgb((byte)(p.GetZ() % 4096), (byte)(p.GetZ() % 4096), (byte)(p.GetZ() % 4096)));
                        }

                        e.Graphics.DrawImage(bmp.Bitmap, 0, 0);
                    }
                }
            }
            else
            {
                Text = "Server unreachable";
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            isServerOnline = service.isServerOnline();
            if (isServerOnline)
            {
                isConnected = service.isKinectOnline();
            }
            else
            {
                isConnected = false;
            }
        }
    }
}
