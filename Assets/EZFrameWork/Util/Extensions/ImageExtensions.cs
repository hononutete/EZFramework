using System.Collections;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EZFramework
{
    public static class ImageExtension
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        public static async Task AlphaFadeOutAsycn(this Image image, float target = 1.0f, float duration = 0.5f)
        {
            await AlphaFadeAsync(image, target, duration);
        }

        public static async Task AlphaFadeAsync(this Image image, float alpha, float duration, Action onFadeComplete = null)
        {
            await image.DOFade(alpha, duration).OnComplete(() => { if (onFadeComplete != null) onFadeComplete(); });
        }

        public static async Task AlphaFadeInAsync(this Image image, float duration = 0.5f)
        {
            await AlphaFadeAsync(image, 0.0f, duration);
        }

    }
}