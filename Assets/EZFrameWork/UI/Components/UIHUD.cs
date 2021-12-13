using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace EZFramework.UI
{

    /// <summary>
    /// ワールド上のオブジェクトに重ねる形で表示するUI要素
    /// </summary>
    public class UIHUD : MonoBehaviour
    {
        public bool updatePosition = true;
        GameObject target;
        public Vector2 offset;
        RectTransform rect;
        RectTransform parentRect;

        public virtual void Init()
        {
            rect = GetComponent<RectTransform>();
            parentRect = transform.parent.GetComponent<RectTransform>();
        }

        public virtual void Update()
        {
            if (updatePosition)
                UpdatePosition();
        }

        void UpdatePosition()
        {
            if (target == null)
                return;

            rect.localPosition = GetRectPosition(parentRect, target) + offset;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
            UpdatePosition();
        }

        public void SetOffset(Vector2 offset) => this.offset = offset;

        public void Show(float duration = 0.0f)
        {
            gameObject.SetActive(true);
            if (duration > 0)
                DOVirtual.DelayedCall(duration, () => Destroy());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public static Vector2 GetRectPosition(RectTransform parentRectTrans, GameObject targetObject)
        {
            if (targetObject == null)
                return Vector2.zero;

            if (UIManager.Instance.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                return RectTransformUtility.WorldToScreenPoint(Camera.main, targetObject.transform.position);

            }
            else if (UIManager.Instance.CanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetObject.transform.position);
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTrans, screenPos, Camera.main, out pos);
                return pos;
            }

            return Vector2.zero;
        }
    }
}