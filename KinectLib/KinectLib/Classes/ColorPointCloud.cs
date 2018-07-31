﻿using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using System;

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

            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var depthIndex = (y * Width) + x;

                    var player = bodyData[depthIndex];

                    if (player != 0xff)
                    {
                        var colorPoint = colorPoints[depthIndex];

                        var colorX = (int) Math.Floor(colorPoint.X + 0.5);
                        var colorY = (int) Math.Floor(colorPoint.Y + 0.5);

                        if ((colorX >= 0) && (colorX < colorWidth) && (colorY >= 0) && (colorY < colorHeight))
                        {
                            var colorIndex = ((colorY * colorWidth) + colorX) * Constants.BYTES_PER_PIXEL;
                            var displayIndex = depthIndex * Constants.BYTES_PER_PIXEL;

                            displayPixels[displayIndex + 0] = colorData[colorIndex];
                            displayPixels[displayIndex + 1] = colorData[colorIndex + 1];
                            displayPixels[displayIndex + 2] = colorData[colorIndex + 2];
                            displayPixels[displayIndex + 3] = 0xff;
                        }
                    }
                }
            }

            DisplayPixels = displayPixels;
        }

        public DateTime TimeStamp { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        //public ushort[] DepthData { get; set; }


        public byte[] DisplayPixels { get; set; }
    }
}