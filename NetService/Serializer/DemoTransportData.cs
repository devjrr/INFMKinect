using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace NetService.Serializer
{
    class DemoTransportData : ISerializer
    {
        private const string prefix = "Data_jpg";
        private const string suffix = ".txt";
        private const int fileCount = 10;

        private const string errorText = @"
Error:
Please place Debug-Data in the same folder as the .exe:

e.g: \INFMKinect\NetService\bin\Debug

The files must be named after following schema(remember to start with 0 and end with 9) :

Data_jpg0.txt
Data_jpg1.txt
Data_jpg2.txt
Data_jpg3.txt
...
Data_jpg9.txt

Do NOT commit Debug-Data to GitHub.";


        private List<byte[]> demoData;
        private int currentIndex = 0;



        private DemoTransportData()
        {
            demoData = new List<byte[]>();
        }

        public static ISerializer ReadfromLocalFiles()
        {
            DemoTransportData dtd = new DemoTransportData();

            for (int i = 0; i < 10; i++)
            {
                string fileName = prefix + i + suffix;
                try
                {
                    byte[] data = dtd.readDemoDataFromFile(fileName);
                    dtd.demoData.Add(data);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(errorText);
                    Console.WriteLine("Exception: " + e.FileName + " not found!");
                }

            }

            return dtd;
        }

        private byte[] readDemoDataFromFile(string fileName)
        {
            string contents = File.ReadAllText(fileName);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(contents);
            string inner = xml.FirstChild.InnerText;

            return Convert.FromBase64String(inner);
        }

        public byte[] getData()
        {
            if (demoData.Count == fileCount)
            {
                byte[] data = demoData[currentIndex];
                currentIndex++;
                currentIndex %= fileCount;
                return data;
            }
            else
            {
                return new byte[] { 0x00 };
            }

        }

        public string getSkeletonData()
        {
            throw new NotImplementedException("Not working in Demo-Mode");
        }
    }
}
