using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace EZFramework.UI
{

    public class UITextNumberCurve : MonoBehaviour
    {
        Text text;
        TextMeshProUGUI tmpro;
        int number;
        float numberF;
        public int padding = 0;
        public Ease ease = Ease.OutQuad;
        public string format = "";
        Tweener tweener;
        void Awake()
        {
            text = GetComponent<Text>();
            tmpro = GetComponent<TextMeshProUGUI>();
        }

        public void SetNumberImmediate(int newNumber)
        {
            number = newNumber;
            UpdateNumber();
        }

        public void SetNumberImmediate(float newNumberF)
        {
            numberF = newNumberF;
            UpdateNumberF();
        }

        public void SetNumber(int newNumber, float duration = 1)
        {
            if (tweener != null)
                tweener.Complete();

            if (duration == 0)
            {
                number = newNumber;
                UpdateNumber();
            }
            else
            {

                tweener = DOTween.To(() => number, (int val) => number = val, newNumber, duration)
                        .SetEase(ease)
                        .OnUpdate(UpdateNumber);
            }
        }

        public async Task SetNumberAsync(int newNumber, float duration = 1)
        {
            if (tweener != null)
                tweener.Complete();

            if (duration == 0)
            {
                number = newNumber;
                UpdateNumber();
            }
            else
            {

                tweener = DOTween.To(() => number, (int val) => number = val, newNumber, duration)
                        .SetEase(ease)
                        .OnUpdate(UpdateNumber);
                await tweener;
            }
        }

        public async Task SetNumberAsync(float newNumber, float duration = 1)
        {
            if (tweener != null)
                tweener.Complete();

            if (duration == 0)
            {
                numberF = newNumber;
                UpdateNumber();
            }
            else
            {

                tweener = DOTween.To(() => numberF, (float val) => numberF = val, newNumber, duration)
                        .SetEase(ease)
                        .OnUpdate(UpdateNumberF);
                await tweener;
            }
        }

        void UpdateNumber()
        {
            if (text != null)
            {
                text.text = GetStr();
            }

            if (tmpro != null)
            {
                tmpro.text = GetStr();
            }
        }

        void UpdateNumberF()
        {
            if (text != null)
            {
                text.text = GetStrF();
            }

            if (tmpro != null)
            {
                tmpro.text = GetStrF();
            }
        }

        string GetStr()
        {
            string numberStr = string.Format("{0}", number).PadLeft(padding, '0');
            if (format != "")
            {
                if (format.Contains("{0}"))
                {
                    return format.Replace("{0}", numberStr);
                }
            }
            return numberStr;
            //return string.Format("{0}", number).PadLeft(padding, '0');
        }

        string GetStrF()
        {
            string numberStr = numberF.ToString("F1");
            if (format != "")
            {
                if (format.Contains("{0}"))
                {
                    return format.Replace("{0}", numberStr);
                }
            }
            return numberStr;
        }
    }
}
