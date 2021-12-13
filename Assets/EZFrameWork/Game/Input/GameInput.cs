using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace EZFramework.Game
{

    /// <summary>
    /// バトル内の入力インターフェイス。デバイス
    /// </summary>
    public class GameInput : MonoBehaviour
    {
        public event Action onNothingPressed;

        public int layerMask = 1;

        private GameObject hitObject;

        public void SetLayerMask(int layerMask)
        {
            this.layerMask = layerMask;
        }


        protected void NothingPressed()
        {
            if (onNothingPressed != null)
                onNothingPressed();
        }

        protected bool IsPointerOnUGUI(Vector2 screenPosition)
        {
            if (EventSystem.current == null) { return false; }

            PointerEventData eventDataCurrent = new PointerEventData(EventSystem.current);
            eventDataCurrent.position = screenPosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventDataCurrent, raycastResults);
            bool result = raycastResults.Count > 0;
            return result;
        }

    }
}
