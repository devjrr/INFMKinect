using Microsoft.Kinect;
using System;
using System.Linq;

namespace KinectLib.Classes
{
    [Serializable]
    public class HighlightedPointCloud
    {

        public HighlightedPointCloud()
        {
            
        }

        public HighlightedPointCloud(DepthFrame iDepthFrame, BodyIndexFrame iBodyIndexFrame)
        {
            TimeStamp = DateTime.Now;

            MinDepth = iDepthFrame.DepthMinReliableDistance;
            MaxDepth = iDepthFrame.DepthMaxReliableDistance;

            Width = iDepthFrame.FrameDescription.Width;
            Height = iDepthFrame.FrameDescription.Height;
            DepthData = new ushort[Width * Height];
            BodyData = new int[Width * Height];

            iDepthFrame.CopyFrameDataToArray(DepthData);

            var bodyData = new byte[Width * Height];
            iBodyIndexFrame.CopyFrameDataToArray(bodyData);
            BodyData = bodyData.Select(x => (int)x).ToArray();
        }

        public DateTime TimeStamp { get; set; }

        public ushort MinDepth { get; set; }

        public ushort MaxDepth { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ushort[] DepthData { get; set; }

        public int[] BodyData { get; set; }

    }
}
