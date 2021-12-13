using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EZFramework.UI
{
    public class UISubContainer : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void MoveOut()
        {
            gameObject.SetActive(false);
            OnMovedOut();
        }

        public void MoveIn()
        {
            gameObject.SetActive(true);
            OnMovedIn();
        }

        protected virtual void OnMovedIn()
        {

        }

        protected virtual void OnMovedOut()
        {

        }
    }
}
