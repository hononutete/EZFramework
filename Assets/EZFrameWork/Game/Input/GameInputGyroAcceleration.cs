using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace EZFramework.Game
{
    public class GameInputGyroAcceleration : MonoBehaviour
    {
        int sampleCount = 1;
        public Queue<Vector3> samples = new Queue<Vector3>();
        public Vector3 acceleration;
        public event Action<Vector3> onAccelerationUpdated;

        public float threshold = 0.1f;
        public const float THRESHOLD_GENERAL = 0.05f;
        public const float THRESHOLD_BREAK = 1.0f;

        void Start()
        {
            SetThreshold(THRESHOLD_GENERAL);
        }

        void Update()
        {
            float x = Mathf.Abs(Input.gyro.userAcceleration.x) < threshold ? 0.0f : Input.gyro.userAcceleration.x;
            float y = Mathf.Abs(Input.gyro.userAcceleration.y) < threshold ? 0.0f : Input.gyro.userAcceleration.y;
            float z = Mathf.Abs(Input.gyro.userAcceleration.z) < threshold ? 0.0f : Input.gyro.userAcceleration.z;

            Vector3 ac = new Vector3(x, y, z);

            //サンプル更新
            if (samples.Count > sampleCount)
                samples.Dequeue();

            samples.Enqueue(ac);

            //サンプラー実行
            acceleration = new Vector3(
                samples.Select((arg) => arg.x).Average(),
                samples.Select((arg) => arg.y).Average(),
                samples.Select((arg) => arg.z).Average()
            );

            if (onAccelerationUpdated != null)
                onAccelerationUpdated(acceleration);
        }

        public void ResetToZero()
        {
            acceleration = Vector3.zero;
            Vector3 v1 = new Vector3(
                samples.Select((arg) => arg.x).Average(),
                samples.Select((arg) => arg.y).Average(),
                samples.Select((arg) => arg.z).Average()
            );

            int count = samples.Count;
            samples.Clear();
            for (int i = 0; i < count; i++)
            {
                samples.Enqueue(Vector3.zero);
            }


            Vector3 v2 = new Vector3(
                samples.Select((arg) => arg.x).Average(),
                samples.Select((arg) => arg.y).Average(),
                samples.Select((arg) => arg.z).Average()
            );
        }

        public void SetThreshold(float threshold, float delay = 0)
        {
            if (delay <= 0)
                this.threshold = threshold;
            else
                StartCoroutine(StartSetThreshold(threshold, delay));
        }

        IEnumerator StartSetThreshold(float value, float delay)
        {
            yield return new WaitForSeconds(delay);
            this.threshold = value;
        }

        public bool IsValid(float value) => Mathf.Abs(value) >= threshold;

        public bool IsValidAcceleration(Vector3 v) => v.sqrMagnitude >= threshold;
    }
}
