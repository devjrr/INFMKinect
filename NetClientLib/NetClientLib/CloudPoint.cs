using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetClientLib
{
    public class CloudPoint
    {
        private float R, G, B, X, Y, Z;

        public CloudPoint(float r, float g, float b, float x, float y, float z)
        {
            R = r;
            G = g;
            B = b;
            X = x;
            Y = y;
            Z = z;
        }

        public float GetB()
        {
            return B;
        }

        public float GetG()
        {
            return G;
        }

        public float GetR()
        {
            return R;
        }

        public float GetX()
        {
            return X;
        }

        public float GetY()
        {
            return Y;
        }

        public float GetZ()
        {
            return Z;
        }


        internal void SetB(float B)
        {
            this.B = B;
        }

        internal void SetG(float G)
        {
            this.G = G;
        }

        internal void SetR(float R)
        {
            this.R = R;
        }

        internal void SetX(float X)
        {
            this.X = X;
        }

        internal void SetY(float Y)
        {
            this.Y = Y;
        }

        internal void SetZ(float Z)
        {
            this.Z = Z;
        }

        internal void SetAll(float r, float g, float b, float x, float y, float z)
        {
            R = r;
            G = g;
            B = b;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
