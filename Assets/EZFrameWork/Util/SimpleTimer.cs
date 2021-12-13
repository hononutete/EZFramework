using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Util
{
    public class SimpleTimer
    {
        SimpleTimerGameComponent timer;
        public bool isOn { get { return timer.isOn; } }

        public SimpleTimer()
        {
            GameObject go = new GameObject("SimpleTimer");
            timer = go.AddComponent<SimpleTimerGameComponent>();
        }

        public void StartTimer(float duration, Action onFinished)
        {

            timer.StartTimer(duration, onFinished);
        }

        public void ResetTimer()
        {
            timer.ResetTimer();
        }

        public float GetLeftTime()
        {
            return timer.leftTime;
        }
    }
}
