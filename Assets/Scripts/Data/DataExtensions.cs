using UnityEngine;

namespace Data
{
    public static class DataExtensions
    {
        public static Vector3Data AsVector3Data(this Vector3 vector) =>
            new Vector3Data(vector.x, vector.y, vector.z);
        
        public static Vector3Data AsVector2Data(this Vector2 vector) =>
            new Vector3Data(vector.x, vector.y, 0);
        
        public static Vector3 AsUnityVector(this Vector3Data vector) =>
            new Vector3(vector.X, vector.Y, vector.Z);
    }
}