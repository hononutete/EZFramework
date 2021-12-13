using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;


namespace EZFramework.UI
{
    public class UISimpleAnimation : MonoBehaviour
    {

        public async Task BounceShow(float duration = 0.1f)
        {
            await transform.DOScale(1.0f, duration).SetEase(Ease.OutElastic);
        }

        public async Task ShrinkHide(float duration = 0.1f)
        {
            await transform.DOScale(0, duration);
        }

        public void Reset()
        {
            transform.localScale = Vector3.zero;
        }
    }
}
