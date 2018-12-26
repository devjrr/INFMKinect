using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using KinectLib.Classes;
using Newtonsoft.Json;

namespace NetService.Serializer
{
    class SingleFrameTransportData : ISerializer
    {
        IKinectData kinectData;

        System.Timers.Timer timer = new System.Timers.Timer();

        private const int widthOutput = 512 / 2;
        private const int heightOutput = 424 / 2;

        private const int widthInput = 512;
        private const int heightInput = 424;



        public SingleFrameTransportData(IKinectData kinectData)
        {
            this.kinectData = kinectData;
        }



        public byte[] getData()
        {
            byte[] result = new byte[widthOutput * heightOutput * 1 + widthOutput * heightOutput * 2];

            byte[] color = kinectData.GetColorData();
            ushort[] depth = kinectData.GetDepthData();

            for (int yInput = 0, yOutput = 0; yInput < heightInput; yInput += 2, yOutput++)
            {
                for (int xInput = 0, xOutput = 0; xInput < widthInput; xInput += 2, xOutput++)
                {
                    int iInput = getIfromXY(xInput, yInput, widthInput);

                    byte colorByte = R8G8B8toR3G3B2(
                        color[iInput * 3 + 2],
                        color[iInput * 3 + 1],
                        color[iInput * 3 + 0]
                    );

                    int iOutput = getIfromXY(xOutput, yOutput, widthOutput);
                    result[iOutput] = colorByte;

                    if (colorByte != 0x00)
                    {
                        result[iOutput * 2 + widthOutput * heightOutput] = shortToHigh(depth[iInput]);
                        result[iOutput * 2 + 1 + widthOutput * heightOutput] = shortToLow(depth[iInput]);
                    }
                }
            }

            return LZ4Codec.WrapHC(result);
        }

        public string getSkeletonData()
        {
            var skeleton = kinectData.GetSkeleton();
            return JsonConvert.SerializeObject(skeleton);
        }

        private byte R8G8B8toR3G3B2(byte r, byte g, byte b)
        {
            byte R3_mask = 0b11100000;
            byte G3_mask = 0b00011100;
            byte B2_mask = 0b00000011;

            byte colbyte = (byte)(
                (r >> 0) & R3_mask
                | (g >> 3) & G3_mask
                | (b >> 6) & B2_mask
            );

            // Black is color key. Therefore only return black, if rgb is too
            if (colbyte == 0x00 && (r != 0x00 || g != 0x00 || b != 0x00))
            {
                colbyte = 0x01;
            }

            return colbyte;
        }

        private byte[] downsize(byte[] arr, int skip)
        {
            byte[] result = new byte[arr.Length / skip];
            for (int i = 0; i < arr.Length; i += skip)
            {

            }
            return result;
        }

        private static ushort bytesToShort(byte a, byte b)
        {
            return (ushort)(a + (b << 8));
        }
        private static byte shortToLow(ushort a)
        {
            return (byte)a;
        }

        private static byte shortToHigh(ushort a)
        {
            return (byte)(a >> 8);
        }

        private int getXfromI(int i, int width)
        {
            return i % width;
        }

        private int getYfromI(int i, int width)
        {
            return i / width;
        }

        private int getIfromXY(int x, int y, int width)
        {
            return y * width + x;
        }
    }
}
