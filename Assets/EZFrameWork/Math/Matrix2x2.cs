using UnityEngine;

namespace EZFramework.Math
{
    public class Matrix2x2
    {
        public float m00;
        public float m01;
        public float m10;
        public float m11;

        public static Matrix2x2 Rotate(float theta)
        {
            Matrix2x2 m = new Matrix2x2();
            m.m00 = Mathf.Cos(theta * Mathf.Deg2Rad);
            m.m01 = -Mathf.Sin(theta * Mathf.Deg2Rad);
            m.m10 = Mathf.Sin(theta * Mathf.Deg2Rad);
            m.m11 = Mathf.Cos(theta * Mathf.Deg2Rad);
            return m;
        }



        public static Vector2 operator *(Matrix2x2 m, Vector2 v)
        {
            return new Vector2(
                v.x * m.m00 + v.y * m.m01,
                v.x * m.m10 + v.y * m.m11
            );
        }
    }
}