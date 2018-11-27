using KinectLib.Interfaces;
using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetService.Serializer
{
    class SingleFrameTransportData : ISerializer
    {
        IKinectData kinectData;

        System.Timers.Timer timer = new System.Timers.Timer();

        private const int width = 512 / 2;
        private const int height = 424 / 2;

        private const int widthInput = 512;
        private const int heightInput = 424;



        public SingleFrameTransportData(IKinectData kinectData)
        {
            this.kinectData = kinectData;
        }



        public byte[] getData()
        {

            byte[] result = new byte[width * height * 1 + width * height * 2];

            byte[] color = kinectData.GetColorData();
            ushort[] depth = kinectData.GetDepthData();
            // Buffer.BlockCopy(kinectData.GetDepthData(), 0, result, width * height, width * height * 2); 



            for (int y = 0, yz = 0; y < heightInput; y += 2, yz++)
            {
                for (int x = 0, xz = 0; x < widthInput; x += 2, xz++)
                {
                    var i = getIfromXY(x, y, widthInput);
                    byte colorByte = R8G8B8toR3G3B3(
                        (color[i * 3 + 2]),
                        (color[i * 3 + 1]),
                        (color[i * 3 + 0])
                    );

                    var iz = getIfromXY(xz, yz, width);
                    result[iz] = colorByte;



                    result[iz * 2 + width * height] = shortToHigh(depth[i]);
                    result[iz * 2 + 1 + width * height] = shortToLow(depth[i]);
                }
            }

            return LZ4Codec.Wrap(result);
        }

        public string getSkeletonData()
        {
            var skeleton = kinectData.GetSkeleton();
            return JsonConvert.SerializeObject(skeleton);
        }

        private byte R8G8B8toR3G3B3(byte r, byte g, byte b)
        {
            byte R3_mask = 0b11100000;
            byte G3_mask = 0b00011100;
            byte B2_mask = 0b00000011;

            return (byte)(
                (r >> 0) & R3_mask
                | (g >> 3) & G3_mask
                | (b >> 6) & B2_mask
            );
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
