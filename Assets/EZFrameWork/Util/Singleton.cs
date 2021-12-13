using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Util
{
    public class Singleton<T> where T : new()
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                //インスタンスが生成されていない場合
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public virtual void Init() { }
        public virtual void Dispose() { _instance = default(T); }
    }
}
