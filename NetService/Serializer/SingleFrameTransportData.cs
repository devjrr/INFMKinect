using KinectLib.Interfaces;
using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetService.Serializer
{
    class SingleFrameTransportData : ISerializer
    {
        IKinectData kinectData;

        System.Timers.Timer timer = new System.Timers.Timer();

        public SingleFrameTransportData(IKinectData kinectData)
        {
            this.kinectData = kinectData;
        }



        public byte[] getData()
        {

            byte[] result = new byte[512 * 424 * 1 + 512 * 424 * 2];

            byte[] color = kinectData.GetColorData();

            Buffer.BlockCopy(kinectData.GetDepthData(), 0, result, 512 * 424, 512 * 424 * 2);

            // Convert R8G8B8 to R3G3B2
            for (int i = 0; i < 512 * 424; i++)
            {
                byte R3_mask = 0b11100000;
                byte G3_mask = 0b00011100;
                byte B2_mask = 0b00000011;

                result[i] = (byte)(
                      (color[i * 3 + 2] >> 0) & R3_mask
                    | (color[i * 3 + 1] >> 3) & G3_mask
                    | (color[i * 3 + 0] >> 6) & B2_mask
                    );
                if (result[i] == 0x00)
                {
                    result[512 * 424 + i * 2] = 0;
                    result[512 * 424 + i * 2 + 1] = 0;
                }
            }


            return LZ4Codec.Wrap(result);
        }
    }
}
