using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game
{
    public class ServiceLocator
    {
        Dictionary<Type, object> list = new Dictionary<Type, object>();

        public T Resolve<T>()
        {
            if (list.ContainsKey(typeof(T)))
            {
                return (T)list[typeof(T)];
            }
            else
            {
                Debug.LogError($"error: no instance located in servicelocator {typeof(T)}");
                return default(T);
            }
        }

        public void Register<T>(T instance)
        {
            if (list.ContainsKey(typeof(T)))
            {
                Debug.LogError($"error: locator already contains insance of {typeof(T)}");
            }
            else
            {
                list.Add(typeof(T), instance);
            }
        }
    }
}
