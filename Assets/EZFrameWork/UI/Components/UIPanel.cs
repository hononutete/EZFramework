using System.Collections;
using System;
using System.Collections.Generic;
//using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace EZFramework.UI
{

    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// 一度でも表示されたことがあるかどうかのフラグ
        /// </summary>
        public bool hasActivatedOnce = false;

        /// <summary>
        /// この画面上に別の画面がpushされるときに、この画面を非表示にするかどうか
        /// </summary>
        public bool deactivateOnPush = true;

        /// <summary>
        /// ロードされた時
        /// </summary>
        public virtual void OnLoaded()
        {

        }

        /// <summary>
        /// ロードされた時
        /// </summary>
        public virtual void OnDispose()
        {

        }

        /// <summary>
        /// 初めて表示されたとき
        /// </summary>
        public virtual void OnActivatedFirstTime()
        {

        }

        /// <summary>
        /// 表示された時
        /// </summary>
        public virtual void OnActivated()
        {

        }

        /// <summary>
        /// 非表示になった時
        /// </summary>
        public virtual void OnDeactivated()
        {

        }

        /// <summary>
        /// stackにpushされた時
        /// </summary>
        public virtual void OnPushed()
        {

        }

        /// <summary>
        /// stackからpopされた時
        /// </summary>
        public virtual void OnPopped()
        {

        }

        /// <summary>
        /// 他のパネルがこのパネルの上にpushされた時
        /// </summary>
        public virtual void OnPushedOut()
        {

        }

        /// <summary>
        /// 他のパネルがPopされて、そのパネルの下にあったこのパネルが表示されるとき
        /// </summary>
        public virtual void OnPoppedIn()
        {

        }

        #region Alpha Layer

        public Image alphaLayer;

        public async UniTask AlphaFadeOutAsycn(float duration = 0.5f)
        {
            if (alphaLayer == null) return;
            await AlphaFadeAsync(0.6f, duration);
        }

        public async UniTask AlphaFadeAsync(float alpha, float duration, Action onFadeComplete = null)
        {
            if (alphaLayer == null) return;
            await alphaLayer.DOFade(alpha, duration).OnComplete(() => { if (onFadeComplete != null) onFadeComplete(); });
        }

        public async UniTask AlphaFadeInAsync(float duration = 0.5f)
        {
            if (alphaLayer == null) return;
            await AlphaFadeAsync(0.0f, duration);
        }

        #endregion

    }
}
