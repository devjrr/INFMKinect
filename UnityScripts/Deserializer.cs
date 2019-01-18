using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

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

        public IList<CloudPoint> deserialzeSkeleton(string data)
        {
            string path;
            string jsonString;


            JObject jObject = JObject.Parse(data);
            IEnumerable<JToken> xCoordinate = jObject.SelectTokens("$..X");
            JToken[] xArrayJTokens = xCoordinate.ToArray();
            var xArray = Array.ConvertAll(xArrayJTokens, item => (float)item);


            IEnumerable<JToken> yCoordinate = jObject.SelectTokens("$..Y");
            JToken[] yArrayJTokens = yCoordinate.ToArray();
            var yArray = Array.ConvertAll(yArrayJTokens, item => (float)item);


            IEnumerable<JToken> zCoordinate = jObject.SelectTokens("$..Z");
            JToken[] zArrayJTokens = zCoordinate.ToArray();
            var zArray = Array.ConvertAll(zArrayJTokens, item => (float)item);

            IList<CloudPoint> points = new List<CloudPoint>(xArray.Length);

            for (int i = 0; i < 25; i++)
            {
                points.Add(new CloudPoint(1.0f, 0, 0, xArray[i], yArray[i], zArray[i]));
            }

            return points;
        }
    }
}
