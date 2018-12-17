using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetService
{
    class Framerate
    {
        private const int SECOND = 1000;

        private int lastTick;
        private int lastFrameRate;
        private int frameRate;
        public void MeasureHere()
        {
            int elapsed = Environment.TickCount - lastTick;
            if (elapsed >= SECOND)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = Environment.TickCount;
            }
            ++frameRate;
        }
        public int FrameRate
        {
            get => lastFrameRate;
        }
    }
}
