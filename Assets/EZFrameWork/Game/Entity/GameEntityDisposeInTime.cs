using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EZFramework.Game
{
    public class GameEntityDisposeInTime : MonoBehaviour
    {
        public float duration = 1.0f;

        void Start()
        {
            DOVirtual.DelayedCall(duration, () =>
            {
                GameEntity e = gameObject.GetComponent<GameEntity>();
                if (e != null)
                    GameEntityDisposer.Dispose(e);
            });
        }

    }
}
