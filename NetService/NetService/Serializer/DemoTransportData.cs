using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NetService.Serializer
{
    class DemoTransportData : ISerializer
    {
        private const string Prefix = "Data_jpg";
        private const string Suffix = ".txt";
        private const int FileCount = 10;

        private const string ErrorText = @"
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

        private const string SkeletonErrorText = @"
Error:
Please place Debug-Data in the same folder as the .exe:

e.g: \INFMKinect\NetService\bin\Debug

The files must be named after following schema(remember to start with 0 and end with 9) :

Data_skeleten0.txt
Data_skeleten1.txt
Data_skeleten2.txt
Data_skeleten3.txt
...
Data_skeleten9.txt

Do NOT commit Debug-Data to GitHub.";


        private readonly List<byte[]> _demoData;
        private readonly List<string> _demoSkeletonData;
        private int _currentIndex;

        private DemoTransportData()
        {
            _demoData = new List<byte[]>();
            _demoSkeletonData = new List<string>();
        }

        public static ISerializer ReadfromLocalFiles()
        {
            var dtd = new DemoTransportData();

            for (var i = 0; i < FileCount; i++)
            {
                var fileName = Prefix + i + Suffix;
                try
                {
                    var data = dtd.ReadDemoDataFromFile(fileName);
                    dtd._demoData.Add(data);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(ErrorText);
                    Console.WriteLine("Exception: " + e.FileName + " not found!");
                }

            }

            for (var i = 0; i < FileCount; i++)
            {
                var fileName = "Data_skeleton" + i + ".json";
                try
                {
                    var data = File.ReadAllText(fileName);
                    /*var xml = new XmlDocument();
                    xml.LoadXml(data);
                    var inner = xml.FirstChild.InnerText;*/

                    dtd._demoSkeletonData.Add(data);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(SkeletonErrorText);
                    Console.WriteLine("Exception: " + e.FileName + " not found!");
                }

            }

            return dtd;
        }

        private byte[] ReadDemoDataFromFile(string fileName)
        {
            var contents = File.ReadAllText(fileName);

            var xml = new XmlDocument();
            xml.LoadXml(contents);
            var inner = xml.FirstChild.InnerText;

            return Convert.FromBase64String(inner);
        }

        public byte[] getData()
        {
            if (_demoData.Count != FileCount) return new byte[] {0x00};

            var data = _demoData[_currentIndex];
            _currentIndex++;
            _currentIndex %= FileCount;
            return data;

        }

        public string getSkeletonData()
        {
            if (_demoSkeletonData.Count != FileCount) return string.Empty;

            var data = _demoSkeletonData[_currentIndex];
            _currentIndex++;
            _currentIndex %= FileCount;
            return data;
        }
    }
}
