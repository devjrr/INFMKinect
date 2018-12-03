namespace KinectLib.Interfaces
{
    public interface IKinectData
    {

        /// <summary>
        /// Contains the information if a pixel is part of the body (512x424)
        /// </summary>
        /// <returns>information if a pixel is a bodypart</returns>
        bool[] GetBodyData();

        /// <summary>
        /// Contains the color information of the image. Every three values build a BGR tripple (512x424x3)
        /// </summary>
        /// <returns>Colorinformation in a byte array</returns>
        byte[] GetColorData();

        /// <summary>
        /// Contains the depth information of the image (512x424)
        /// </summary>
        /// <returns>Depth data as ushort[]</returns>
        ushort[] GetDepthData();

        /// <summary>
        /// returns the skeleton data as IBodyWrapper
        /// </summary>
        /// <returns>Skeleton Data as IBodyWrapper</returns>
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
