﻿using LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace NetClientLib
{
    class RemoteService : IRemoteService
    {
        System.Net.WebClient client = new System.Net.WebClient();

        private LoadCounter download = new LoadCounter();
        private LoadCounter upload = new LoadCounter();



        public RemoteService()
        {

        }



        public IList<CloudPoint> GetCloudpoints()
        {
            string inner = HtmlGet("Data");
            if (inner == null)
            {
                return null;
            }

            byte[] vs = LZ4Codec.Unwrap(Convert.FromBase64String(inner));


            IList<CloudPoint> points = new List<CloudPoint>(512 * 424);
            for (int i = 0; i < 512 * 424; i++)
            {
                if (vs[i] != 0x00)
                {
                    // Convert R3G3B2 to R8G8B8
                    byte color = vs[i];
                    byte mask = 0b11100000;

                    float r = color & mask;
                    float g = (color << 3) & mask;
                    float b = (color << 6) & mask;


                    float x = i % 512;
                    float y = i / 512;
                    float z = bytesToShort(vs[i * 2 + (512 * 424)], vs[i * 2 + (512 * 424) + 1]);

                    points.Add(new CloudPoint(r, g, b, x, y, z));

                }
            }

            return points;
        }

        private static ushort bytesToShort(byte a, byte b)
        {
            return (ushort)(a + (b << 8));
        }

        public float GetCurrentDownload()
        {
            return download.Load;
        }

        public float GetCurrentUpload()
        {
            return upload.Load;
        }

        public bool isServerOnline()
        {
            return HtmlGet("Status") != null;

        }

        public bool isKinectOnline()
        {
            return HtmlGet("Status").Equals("online");
        }

        private string HtmlGet(string service)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:56789/" + service);
            request.Method = "GET";
            request.ContentType = "text/plain";
            string test = string.Empty;

            long length = request.ContentLength + request.Headers.ToByteArray().Length;
            upload.MeasureHere(length / 1024.0f);
            string inner = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength + response.Headers.ToByteArray().Length;
                    download.MeasureHere(length / 1024.0f);

                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    test = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(test);
                inner = xml.FirstChild.InnerText;
            }
            catch (WebException)
            {

            }

            return inner;
        }
    }
}