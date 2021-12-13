using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EZFramework.Util
{
    public class Shaker : MonoBehaviour
    {
        public float duration = 0.5f;
        public float strength = 1;
        public int vibrato = 10;
        public int randomness = 90;

        public void Shake()
        {
            transform.DOShakePosition(duration, strength, vibrato, randomness);
        }

    }
}
