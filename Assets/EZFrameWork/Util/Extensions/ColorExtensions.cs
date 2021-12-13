using UnityEngine;

namespace EZFramework
{
    public static class ColorExtensions
    {
        public static Vector3 ToVector3(this Color color) => new Vector3(color.r, color.g, color.b);

    }
}
