using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Util
{
    public class DestroyInTime : MonoBehaviour
    {
        public float duration = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(this.gameObject, duration);
        }


    }
}
