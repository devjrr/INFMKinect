using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetClientLib
{
    class LoadCounter
    {
        private const int SECOND = 1000;

        private int lastTick;
        private float lastLoad;
        private float load;

        public void MeasureHere(float addLoad)
        {
            int elapsed = Environment.TickCount - lastTick;
            if (elapsed >= SECOND)
            {
                lastLoad = load;
                load = 0;
                lastTick = Environment.TickCount;
            }

            load += addLoad;
        }

        public float Load
        {
            get { return lastLoad; }
        }
    }
}