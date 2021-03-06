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

        private LoadCounter download = new LoadCounter();
        private LoadCounter upload = new LoadCounter();
        private int width;
        private int height;


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


            width = 256;
            height = 212;
            IList<CloudPoint> points = new List<CloudPoint>(width * height);
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
                    float z = bytesToShort(vs[i * 2 + (width * height) + 1], vs[i * 2 + (width * height)]);

                    points.Add(new CloudPoint(r / 256, g / 256, b / 256, x, y, z));
                }
            }

            return points;
        }

        private static ushort bytesToShort(byte a, byte b)
        {
            return (ushort) (a + (b << 8));
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
            //HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://192.168.0.185:80/" + service);
             HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:56789/" + service);
            request.Method = "GET";
            request.ContentType = "text/plain";
            string test = string.Empty;

            long length = request.ContentLength + request.Headers.ToByteArray().Length;
            upload.MeasureHere(length / 1024.0f);
            string inner = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
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