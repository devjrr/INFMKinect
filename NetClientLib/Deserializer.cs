using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetClientLib
{
    class Deserializer
    {
        public IList<CloudPoint> deserialze(string data, int height, int width)
        {
            byte[] vs = LZ4Codec.Unwrap(Convert.FromBase64String(data));


            IList<CloudPoint> points = new List<CloudPoint>(height * width);
            for (int i = 0; i < width * height; i++)
            {
                if (vs[i] != 0x00)
                {
                    // Convert R3G3B2 to R8G8B8
                    byte color = vs[i];
                    byte mask = 0xE0;

                    float r = color & mask;
                    float g = (color << 3) & mask;
                    float b = (color << 6) & mask;


                    float x = i % width;
                    float y = i / width;
                    float z = bytesToShort(vs[i * 2 + width * height + 1], vs[i * 2 + width * height]);

                    points.Add(new CloudPoint(r / 256, g / 256, b / 256, x, y, z));

                }
            }

            return points;
        }

        private static ushort bytesToShort(byte a, byte b)
        {
            return (ushort)(a + (b << 8));
        }
    }
}
