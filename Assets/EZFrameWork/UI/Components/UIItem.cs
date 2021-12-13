using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;


namespace EZFramework.UI
{

    [RequireComponent(typeof(Button))]
    public class UIItem : MonoBehaviour
    {
        public Image bg;
        public Image frame;
        public Image main;
        public Text text;
        public TextMeshProUGUI tmproText;
        protected Button btn;
        public event UnityAction<UIItem> onBtnClickedOnce;
        public event UnityAction<UIItem> onBtnClicked;
        public event UnityAction<UIItem> onLongTapped;
        public UIItemLongTap longTap;

        public virtual void Init()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(OnClickedHandler);
            longTap = GetComponent<UIItemLongTap>();
        }

        public void SetLongTapEnabled(bool isEnabled)
        {
            if (longTap == null) return;
            longTap.enabled = isEnabled;
            if (isEnabled)
                longTap.onLongTapped += OnLongTapped;
            else
                longTap.onLongTapped -= OnLongTapped;
        }

        void OnLongTapped()
        {
            if (onLongTapped != null)
                onLongTapped(this);
        }

        public void SetBg(Sprite sprite)
        {
            if (bg != null)
                bg.sprite = sprite;
        }

        public void SetFrame(Sprite sprite)
        {
            if (frame != null)
                frame.sprite = sprite;
        }

        public void SetMain(Sprite sprite)
        {
            if (main != null)
                main.sprite = sprite;
        }

        public void SetText(string txt)
        {
            if (text != null)
                text.text = txt;
            else if (tmproText != null)
                tmproText.text = txt;
        }

        void OnClickedHandler()
        {
            if (onBtnClicked != null)
            {
                onBtnClicked(this);
            }

            if (onBtnClickedOnce != null)
            {
                onBtnClickedOnce(this);
                onBtnClickedOnce = null;
            }
        }
    }
}