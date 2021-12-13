using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Util
{
    public class SingletonMonobehaviour<T> : MonoBehaviour, IDisposable where T : Component
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                //インスタンスが生成されていない場合
                if (_instance == null)
                {
                    //シーン上で既にロードされている場合はそれを取得
                    _instance = UnityEngine.Object.FindObjectOfType<T>();

                    //新たにインスタンス化
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).ToString());
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public virtual void Dispose()
        {
            _instance = null;
        }

    }
}
