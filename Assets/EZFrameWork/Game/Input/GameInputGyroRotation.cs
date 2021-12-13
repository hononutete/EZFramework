using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace EZFramework.Game
{
    public class GameInputGyroRotation : GameInput
    {
        int sampleCount = 1;
        public Queue<Quaternion> samples = new Queue<Quaternion>();
        public Quaternion acceleration;
        public event Action<Quaternion> onRotationUpdated;

        /// <summary>
        /// スマホの画面右x、画面上z、画面法線y
        /// </summary>
        public event Action<Quaternion> onRawRotationUpdated;


        public float threshold = 0.1f;
        public const float THRESHOLD_GENERAL = 0.05f;
        public const float THRESHOLD_BREAK = 1.0f;

        public Quaternion CurrentRawRotation => GetRawCoordRotation();
        void Start()
        {
            Input.gyro.enabled = true;
        }


        void Update()
        {

            //端末座標空間とワールド座標空間を合わせる場合
            if (onRotationUpdated != null)
            {
                Quaternion newFixedQ = GetWorldCoordFixedRotation();

                //サンプル更新
                if (samples.Count > sampleCount)
                    samples.Dequeue();

                samples.Enqueue(newFixedQ);

                //サンプラー実行
                acceleration = new Quaternion(
                    samples.Select((arg) => arg.x).Average(),
                    samples.Select((arg) => arg.y).Average(),
                    samples.Select((arg) => arg.z).Average(),
                    samples.Select((arg) => arg.w).Average()
                );

                onRotationUpdated(acceleration);
            }

            //回転量を直接使う場合
            if (onRawRotationUpdated != null)
            {
                Quaternion newRawQ = GetRawCoordRotation();
                onRawRotationUpdated(newRawQ);
            }
        }

        /// <summary>
        /// カメラ向きと同期したい場合
        /// </summary>
        Quaternion GetWorldCoordFixedRotation()
        {
            Quaternion q = Input.gyro.attitude;
            Quaternion newQ = new Quaternion(-q.x, -q.z, -q.y, q.w);

            newQ *= Quaternion.Euler(90f, 0f, 0f);
            return newQ;
        }

        Quaternion GetRawCoordRotation()
        {
            Quaternion q = Input.gyro.attitude;
            Quaternion newQ = new Quaternion(-q.x, -q.z, q.y, q.w);

            return newQ;
        }

    }
}