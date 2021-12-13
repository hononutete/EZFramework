using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace EZFramework.UI
{

    public class UIItemLongTap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float time = 1.0f;
        float timeStamp;
        bool isPressed;
        public event Action onLongTapped;

        public void OnPointerDown(PointerEventData _eventData)
        {
            isPressed = true;
            timeStamp = Time.time;
        }

        public void OnPointerUp(PointerEventData _eventData)
        {
            isPressed = false;
        }

        void Update()
        {
            if (isPressed && Time.time - timeStamp >= time)
            {
                if (onLongTapped != null)
                    onLongTapped();
                isPressed = false;
            }
        }
    }
}
