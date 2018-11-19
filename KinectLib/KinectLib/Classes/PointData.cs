using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectLib.Classes
{
    public class PointData
    {
        public int X { get; set; }

        public int Y { get; set; }

        public bool Person { get; set; }

        public ushort Depth { get; set; }

        public string Color { get; set; }


    }
}
