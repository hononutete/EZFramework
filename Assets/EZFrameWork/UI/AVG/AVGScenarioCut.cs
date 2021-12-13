using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EZFramework.Game;

namespace EZFramework.AVG
{

    public class AVGScenarioCut
    {
        public enum ETextDisplayType
        {
            AT_ONCE = 1,
            SCROLL = 2
        }

        public MAvgScenarioCut cutData { get; private set; }
        Sprite character;
        Sprite frame;
        string text;

        public AVGScenarioCut(MAvgScenarioCut cutData)
        {
            this.cutData = cutData;
        }

        public async UniTask Load()
        {
            text = ServiceLocatorProvider.Instance.Current.Resolve<Language>().GetText(cutData.TextKey);

            //キャラ画像リソースをロード
            if (cutData.CharacterResourceName != string.Empty)
            {
                if (AVG.imageLoader != null)
                    character = await AVG.imageLoader.LoadCharacterImage(AVG.characterImagePath);
            }

            //prefabをロード、一枚画像のパターンもあれば、9patchを使う可能性もある。
            if (cutData.FrameType != string.Empty)
            {
                frame = await AVG.imageLoader.LoadFrameImage(AVG.frameImagePath);
            }
        }

        public void ClearText()
        {
            AVG.view.SetText(string.Empty);
        }

        public void ClearCharacter()
        {
            AVG.view.SetImage(null);
        }

        public void Apply()
        {
            //TODO: add field to avg master
            switch (ETextDisplayType.SCROLL)
            {
                case ETextDisplayType.AT_ONCE:
                    AVG.view.SetText(text);
                    break;
                case ETextDisplayType.SCROLL:
                    AVG.view.SetStreamText(text, 0.1f);
                    break;
                default:
                    break;
            }
            AVG.view.SetWaitForTapToEnd(true);//TODO:debug
                                              //AVG.view.SetWaitForTapToEnd((AVGScenario.EAIStateTransitionConditionType)cutData.NextConditionType == AVGScenario.EAIStateTransitionConditionType.TAP);
            AVG.view.SetFrame(frame);
            AVG.view.SetImage(character);
        }

        public void Open()
        {
            AVG.view.dialog.Show();
        }

        public void Close()
        {
            AVG.view.dialog.Hide();
        }
    }
}
