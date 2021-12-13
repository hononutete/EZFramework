using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EZFramework.Util
{
    public class SimpleTimerGameComponent : MonoBehaviour
    {
        public float leftTime { get; private set; }
        public bool isOn { get; private set; }
        public bool autoDestroy = true;

        Action onFinished = null;

        // Start is called before the first frame update
        public void StartTimer(float duration, Action onFinished)
        {
            isOn = true;
            leftTime = duration;
            this.onFinished = onFinished;
        }

        public void ResetTimer()
        {
            isOn = false;
        }

        public void SetIsOn(bool isOn)
        {
            this.isOn = isOn;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isOn)
                return;

            leftTime -= Time.deltaTime;

            if (leftTime <= 0f)
            {
                // prevent negative values
                leftTime = 0f;

                if (onFinished != null)
                    onFinished();

                if (autoDestroy)
                    Destroy(this.gameObject);
            }
        }
    }
}
