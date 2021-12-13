using UnityEngine;

namespace EZFramework.Util
{
    public class BillBoard : MonoBehaviour
    {
        void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}