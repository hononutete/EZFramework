using UnityEngine;

namespace EZFramework
{
    public static class Vector3Extension
    {

        public static Vector3 XZ(this Vector3 v) => new Vector3(v.x, 0, v.z);

        public static Vector3 XY(this Vector3 v) => new Vector3(v.x, v.y, 0);

        public static Vector3 YZ(this Vector3 v) => new Vector3(0, v.y, v.z);

        public static Vector2 Vector2XZ(this Vector3 v) => new Vector2(v.x, v.z);

        public static Vector2 Vector2XY(this Vector3 v) => new Vector2(v.x, v.y);

        public static Vector2 Vector2YZ(this Vector3 v) => new Vector2(v.y, v.z);

        public static Vector2Int ToInt(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);

    }
}

