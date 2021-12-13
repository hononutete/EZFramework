using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using EZFramework.Util;

namespace EZFramework.UI
{

    /// <summary>
    /// よくあるユーザーに確認をとるような小さなウィンドウをダイヤログとする。 デフォルトでyesとnoのボタンのクリックイベントを実装。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    //[RequireComponent(typeof(GameAnimationEventHandler))]
    public class UIDialog : MonoBehaviour
    {
        Animator anim;
        Action onFinished;

        public Button yesBtn;
        public Button noBtn;
        public Button closeBtn;

        public event Action onYesClicked;
        public event Action onNoClicked;
        public event Action onCloseClicked;
        public Action onDialogClosedOnce;

        const string ANIM_TRIGGER_SHOW = "show";
        const string ANIM_TRIGGER_HIDE = "hide";

        protected bool isDlgScaleAnimating = false;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            anim = GetComponent<Animator>();
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

            if (yesBtn != null)
                yesBtn.onClick.AddListener(OnYesBtnClicked);
            if (noBtn != null)
                noBtn.onClick.AddListener(OnNoBtnClicked);
            if (closeBtn != null)
                closeBtn.onClick.AddListener(OnCloseBtnClicked);

            transform.Find("frame").localScale = Vector3.zero;
        }



        /// <summary>
        /// アニメーションで表示
        /// </summary>
        public void Show(Action onFinished = null)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            OnShow();

            isDlgScaleAnimating = true;
            anim.SetTrigger(ANIM_TRIGGER_SHOW);

            this.onFinished = onFinished;
            StartCoroutine(WaiForAnimationEnd());
        }

        /// <summary>
        /// アニメーションで非表示
        /// </summary>
        public void Hide(Action onFinished = null)
        {
            isDlgScaleAnimating = true;
            anim.SetTrigger(ANIM_TRIGGER_HIDE);

            this.onFinished = onFinished;
            this.onFinished += () => gameObject.SetActive(false);
            StartCoroutine(WaiForAnimationEnd());
        }

        IEnumerator WaiForAnimationEnd()
        {
            yield return new WaitForAnimationEnd(anim, 0);
            if (onFinished != null)
            {
                onFinished();
                onFinished = null;
            }
            isDlgScaleAnimating = false;
        }

        protected virtual void OnShow() { }

        protected virtual void OnClosed()
        {
            if (onDialogClosedOnce != null)
            {
                onDialogClosedOnce();
                onDialogClosedOnce = null;
            }
        }

        protected virtual void OnYesBtnClicked()
        {
            Hide();

            OnClosed();

            if (onYesClicked != null)
            {
                onYesClicked();
            }

            ClearButtonEvents();
        }

        protected virtual void OnNoBtnClicked()
        {
            Hide();

            OnClosed();

            if (onNoClicked != null)
            {
                onNoClicked();
            }

            ClearButtonEvents();
        }

        protected virtual void OnCloseBtnClicked()
        {
            Hide();

            OnClosed();

            if (onCloseClicked != null)
            {
                onCloseClicked();
            }

            ClearButtonEvents();
        }

        void ClearButtonEvents()
        {
            onYesClicked = null;
            onNoClicked = null;
            onCloseClicked = null;
        }
    }
}


