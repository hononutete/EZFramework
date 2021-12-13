using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework
{
    public static class ActionExtension
    {
        public static void NullSafe(this Action action)
        {
            if (action != null)
                action();
        }

    }

    public static class ListExtensions
    {

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}