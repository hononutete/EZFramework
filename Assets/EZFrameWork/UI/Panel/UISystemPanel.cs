using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace EZFramework.UI
{
    public class UISystemPanel : UIPanel
    {
        public Image mask;
        public Text loadMsg;
        public Image trimMaskUp;
        public Image trimMaskRight;
        public Image trimMaskLeft;
        public Image trimMaskDown;
        public Button screenButton;

        public void ShowLoadingSystemMsg(string msg)
        {
            loadMsg.text = msg;
            loadMsg.gameObject.SetActive(true);
        }

        public void HideLoadingSystemMsg()
        {
            loadMsg.text = "";
            loadMsg.gameObject.SetActive(false);
        }

        public void Fade(float alpha, float duration, Action onFadeComplete = null)
        {
            mask.DOFade(alpha, duration).OnComplete(() => { if (onFadeComplete != null) onFadeComplete(); });
        }

        public void SetMaskAlpha(float alpha)
        {
            Color col = mask.color;
            col.a = alpha;
            mask.color = col;
        }

        public void FadeOut(float duration = 0.5f, Action onFadeComplete = null)
        {
            Fade(1.0f, duration, onFadeComplete);
        }

        public IEnumerator FadeOut(float duration = 0.5f)
        {
            bool f = false;
            FadeOut(duration, () => f = true);
            while (!f) yield return null;
        }

        public void FadeIn(float duration = 0.5f, Action onFadeComplete = null)
        {
            Fade(0.0f, duration, onFadeComplete);
        }

        public IEnumerator FadeIn(float duration = 0.5f)
        {
            bool f = false;
            FadeIn(duration, () => f = true);
            while (!f) yield return null;
        }

        public async Task FadeOutAsync(float duration = 0.5f)
        {
            await FadeAsync(1.0f, duration);
        }

        public async Task FadeAsync(float alpha, float duration, Action onFadeComplete = null)
        {
            await mask.DOFade(alpha, duration).OnComplete(() => { if (onFadeComplete != null) onFadeComplete(); });
        }

        public async Task FadeInAsync(float duration = 0.5f)
        {
            await FadeAsync(0.0f, duration);
        }

        public void UpdateTrimmingMask()
        {
            CanvasScaler canvasScaler = UIManager.Instance.GetComponent<CanvasScaler>();
            float x = canvasScaler.referenceResolution.x / 2.0f;
            float y = canvasScaler.referenceResolution.y / 2.0f;
            trimMaskUp.transform.localPosition = new Vector3(0, y, 0);
            trimMaskRight.transform.localPosition = new Vector3(x, 0, 0);
            trimMaskLeft.transform.localPosition = new Vector3(-x, 0, 0);
            trimMaskDown.transform.localPosition = new Vector3(0, -y, 0);

            //余白がある分を拡大縮小

            if (canvasScaler.matchWidthOrHeight == 0)
            {

            }
            else if (canvasScaler.matchWidthOrHeight == 1)
            {

            }
        }

    }
}
