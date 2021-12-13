using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EZFramework.Game
{
    /// <summary>
    /// UI以外のオブジェクトに対するタッチ入力
    /// </summary>
    public class GameInputTouch : GameInput
    {
        public float minSwipeThreshold = 1;
        public float minTapDistanceThreshold = 0.1f;
        public float minTapTimeThreshold = 0.1f;
        public event Action<Vector2> onTouchBegun;
        public event Action<Vector2> onTouchEnded;
        /// <summary>
        /// arg1: current touch position, arg2: (current touch position) - (touch start position)
        /// </summary>
        public event Action<Vector2, Vector2, Vector2> onTouchMoved;
        public event Action<Vector2> onSwiped;
        public event Action onTapped;

        public event Action onUpdate;

        public Vector2 touchStartPos { get { return touches[Input.GetTouch(0).fingerId].touchStartPos; } }
        public Vector2 touchEndPos { get { return touches[Input.GetTouch(0).fingerId].touchEndPos; } }

        public Dictionary<int, TouchObject> touches = new Dictionary<int, TouchObject>();

        public bool hasTouch => Input.touchCount > 0;

        public Vector2 touchPos => Input.GetTouch(0).position;

        void Update()
        {
            ApplyEachTouch();

            if (onUpdate != null)
                onUpdate();
        }


        void ApplyEachTouch()
        {

            foreach (Touch touch in Input.touches)
                ApplyTouch(touch);
        }

        void ApplyTouch(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (!IsPointerOnUGUI(touch.position))
                {
                    //register new touch
                    if (!touches.ContainsKey(touch.fingerId))
                        touches.Add(touch.fingerId, new TouchObject());


                    touches[touch.fingerId].touchStartPos = touch.position;
                    touches[touch.fingerId].timeStamp = Time.time;

                    if (onTouchBegun != null)
                        onTouchBegun(touch.position);
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touches.ContainsKey(touch.fingerId))
                {
                    if (onTouchMoved != null)
                    {
                        onTouchMoved(touch.position, touch.position - touches[touch.fingerId].touchStartPos, touch.deltaPosition);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (touches.ContainsKey(touch.fingerId))
                {
                    //スワイプの検出
                    touches[touch.fingerId].touchEndPos = touch.position;
                    Vector2 dist = touches[touch.fingerId].touchEndPos - touches[touch.fingerId].touchStartPos;
                    if (dist.sqrMagnitude >= minSwipeThreshold)
                    {
                        if (onSwiped != null)
                            onSwiped(dist);
                    }

                    //タップの検出
                    if (dist.sqrMagnitude <= minTapDistanceThreshold && Time.time - touches[touch.fingerId].timeStamp < minTapTimeThreshold)
                    {
                        if (onTapped != null)
                            onTapped();
                    }

                    if (onTouchEnded != null)
                        onTouchEnded(touch.position);

                    //remove touch
                    touches.Remove(touch.fingerId);
                }

            }
        }


        public class TouchObject
        {
            public Vector2 touchStartPos;
            public Vector2 touchEndPos;
            public float timeStamp;
        }


    }
}
