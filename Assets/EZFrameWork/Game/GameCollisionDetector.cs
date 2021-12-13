using System;
using UnityEngine;

namespace EZFramework.Game
{

    public class GameCollisionDetector : MonoBehaviour
    {
        public event Action<Collider2D> onTriggerEntered2D;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (onTriggerEntered2D != null)
                onTriggerEntered2D(col);
        }
    }
}