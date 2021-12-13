using System.Collections;
using System;
using UnityEngine;

namespace EZFramework.Util
{
    public class WaitForEndOfFrame : MonoBehaviour
    {
        public static WaitForEndOfFrame Create()
        {
            GameObject instance = new GameObject("WaitForEndOfFrame");
            return instance.AddComponent<WaitForEndOfFrame>();
        }

        Action onFinished;

        public void Wait(Action onFinished)
        {
            this.onFinished = onFinished;
            StartCoroutine(Start());
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (onFinished != null)
            {
                onFinished();
                onFinished = null;
            }

            Destroy(this.gameObject);
        }
    }
}
