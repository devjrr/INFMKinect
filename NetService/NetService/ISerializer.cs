﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetService
{
    interface ISerializer
    {
        byte[] getData();

        string getSkeletonData();
    }
}