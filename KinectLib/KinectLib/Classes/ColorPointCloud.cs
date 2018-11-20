using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace KinectLib.Classes
{
    [Serializable]
    public class ColorPointCloud
    {
        public ColorPointCloud()
        {
            
        }

        public ColorPointCloud(ColorFrame iColorFrame, DepthFrame iDepthFrame, BodyIndexFrame iBodyIndexFrame, CoordinateMapper iCoordinateMapper)
        {
            TimeStamp = DateTime.Now;

            Width = iDepthFrame.FrameDescription.Width;
            Height = iDepthFrame.FrameDescription.Height;
            var depthData = new ushort[Width * Height];
            //BodyData = new int[Width * Height];

            iDepthFrame.CopyFrameDataToArray(depthData);

            var bodyData = new byte[Width * Height];
            iBodyIndexFrame.CopyFrameDataToArray(bodyData);

            var colorWidth = iColorFrame.FrameDescription.Width;
            var colorHeight = iColorFrame.FrameDescription.Height;
            var colorData = new byte[colorWidth * colorHeight * Constants.BYTES_PER_PIXEL];
            if (iColorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                iColorFrame.CopyRawFrameDataToArray(colorData);
            }
            else
            {
                iColorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Bgra);
            }

            var colorPoints = new ColorSpacePoint[Width * Height];
            iCoordinateMapper.MapDepthFrameToColorSpace(depthData, colorPoints);

            var displayPixels = new byte[Width * Height * Constants.BYTES_PER_PIXEL];
            Array.Clear(displayPixels, 0, displayPixels.Length);

            var bodyDataBool = new bool[Width * Height];
            var colorDataWithoutAlpha = new byte[Width * Height * 3];

            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {

                    var depthIndex = (y * Width) + x;

                    var player = bodyData[depthIndex];

                    if (player != 0xff)
                    {
                        bodyDataBool[depthIndex] = true;

                        var colorPoint = colorPoints[depthIndex];

                        var colorX = (int) Math.Floor(colorPoint.X + 0.5);
                        var colorY = (int) Math.Floor(colorPoint.Y + 0.5);

                        if ((colorX >= 0) && (colorX < colorWidth) && (colorY >= 0) && (colorY < colorHeight))
                        {
                            var colorIndex = ((colorY * colorWidth) + colorX) * Constants.BYTES_PER_PIXEL;
                            var displayIndex = depthIndex * Constants.BYTES_PER_PIXEL;

                            displayPixels[displayIndex + 0] = colorData[colorIndex]; // B
                            displayPixels[displayIndex + 1] = colorData[colorIndex + 1]; // G
                            displayPixels[displayIndex + 2] = colorData[colorIndex + 2]; // R
                            displayPixels[displayIndex + 3] = 0xff; // Alpha

                            var colorDataIndex = depthIndex * 3;
                            colorDataWithoutAlpha[colorDataIndex + 0] = colorData[colorIndex + 0];
                            colorDataWithoutAlpha[colorDataIndex + 1] = colorData[colorIndex + 1];
                            colorDataWithoutAlpha[colorDataIndex + 2] = colorData[colorIndex + 2];

                        }
                    }
                    
                }
            }

            DisplayPixels = displayPixels;

            BodyData = bodyDataBool;
            DepthData = depthData;
            ColorData = colorDataWithoutAlpha;
        }

        public DateTime TimeStamp { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public byte[] DisplayPixels { get; set; }

        public ushort[] DepthData { get; set; }

        public byte[] ColorData { get; set; }

        public bool[] BodyData { get; set; }
    }
}
