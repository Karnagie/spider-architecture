using System;

namespace Data
{
    [Serializable]
    public class Vector3Data
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3Data(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3Data operator +(Vector3Data first, Vector3Data second) => 
            new(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
    }
}