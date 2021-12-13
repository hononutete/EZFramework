using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace EZFramework.UI
{
    public class UISliderCurve : MonoBehaviour
    {
        public float duration = 0.3f;
        Slider slider;
        Tweener tween;

        void Awake()
        {
            slider = GetComponent<Slider>();

        }

        public async Task SetValueAsync(float value, float duration = 0.3f)
        {
            if (tween != null && !tween.IsComplete())
                tween.Complete();

            tween = DOTween.To(() => slider.value, (v) => slider.value = v, value, duration);
            await tween;
        }

        public void Reset()
        {
            slider.value = 0;
        }
    }
}
