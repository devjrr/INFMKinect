using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectLib.Classes;

namespace KinectLib.Interfaces
{
    public interface IKinectData
    {

        /***
         * Contains the information if a pixel is part of the body (512x424)
         */
        bool[] GetBodyData();

        /***
         * Contains the color information of the image. Every three values build a BGR tripple (512x424x3)
         */
        byte[] GetColorData();

        /***
         * Contains the depth information of the image (512x424)
         */
        ushort[] GetDepthData();

        /***
         * returns the skeleton data as IBodyWrapper
         */
        IBodyWrapper GetSkeleton();

        /// <summary>
        /// Shuts the KinectService down. Must be called on the end of the program.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Checks if a kinect is connected.
        /// </summary>
        /// <returns>true if a kinect is connected</returns>
        bool IsKinectConnected();

    }
}
