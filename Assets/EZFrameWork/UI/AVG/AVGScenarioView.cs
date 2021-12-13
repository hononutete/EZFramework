using System;
using UnityEngine;
using UnityEngine.UI;
using EZFramework.UI;
using EZFramework.Util;

namespace EZFramework.AVG
{
    public class AVGScenarioView : UIPanel
    {
        public Button touchDetector;
        public UIDialog dialog;
        public Text text;
        public Image characterImage;
        public Image frameImage;
        bool waitForTap = false;
        public bool IsShowTextComplete { get; private set; }

        public event Action onTapped;


        TextStream textStream = new TextStream();
        Scheduler scheduler = new Scheduler();

        public string fullText;

        public void SetImage(Sprite sprite)
        {
            characterImage.sprite = sprite;
        }

        public void SetText(string txt)
        {
            text.text = txt;
            IsShowTextComplete = true;

            //スケジューラーは使用しない
            scheduler.SetInterval(-1);
        }

        public void SetFrame(Sprite sprite)
        {
            frameImage.sprite = sprite;
        }

        public void SetStreamText(string text, float interval)
        {
            fullText = text;

            textStream.Write(text);
            scheduler.SetInterval(interval);
        }

        public void SetWaitForTapToEnd(bool waitForTap)
        {
            this.waitForTap = waitForTap;
        }

        public void OnClicked()
        {
            //テキスト表示が完了していない場合、即時表示
            if (!IsShowTextComplete)
            {
                SetText(fullText);
            }
            else if (waitForTap)
            {
                if (onTapped != null)
                    onTapped();
            }
        }

        void Update()
        {
            //次の文字を表示するスケジューラー待機
            if (scheduler.Check())
            {
                string addText = textStream.ReadNext();

                if (addText != string.Empty)
                    text.text += addText;
                else
                {
                    IsShowTextComplete = true;
                }
            }
        }

    }
}

