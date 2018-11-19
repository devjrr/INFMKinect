using System;
using System.Text;
using Microsoft.Kinect;

namespace KinectLib.KinectHelper
{
    public static class KinectHelper
    {
        public static string ByteArrayToHex(Byte[] iArray)
        {
            var sb = new StringBuilder(iArray.Length * 2);
            foreach (var b in iArray)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
