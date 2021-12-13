using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EZFramework.UI
{
    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// 瞬時に表示する
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 瞬時に表示する
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
