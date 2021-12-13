using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace EZFramework.AVG
{
    public class AVGScenario
    {
        public enum EAIStateTransitionConditionType
        {
            NONE = 0,
            FORCE = 1,
            TAP = 2,
        }

        int index = -1;
        public List<AVGScenarioCut> cuts = new List<AVGScenarioCut>();
        List<MAvgScenarioCut> cutDatas;
        public AVGScenarioCut currentCut { get; private set; }
        Action onTransitionConditionFullfilled;
        const float INTERVAL = 0.3f;

        public AVGScenario(List<MAvgScenarioCut> cutDatas)
        {
            this.cutDatas = cutDatas;
        }

        public async UniTask Load()
        {
            foreach (MAvgScenarioCut cutData in cutDatas)
            {
                AVGScenarioCut cut = new AVGScenarioCut(cutData);
                await cut.Load();
                cuts.Add(cut);
            }
        }

        public bool Next()
        {
            //一番最初のカットは最初のインデックス
            if (currentCut == null)
            {
                currentCut = cuts[0];
                currentCut.Apply();
                currentCut.Open();
                return true;
            }
            else
            {
                //最後のカットだった場合
                if (currentCut.cutData.NextId == 0)
                {
                    currentCut.Close();
                    return false;
                }
                //次がある場合遷移
                else
                {
                    currentCut = cuts.FirstOrDefault(e => e.cutData.ID == currentCut.cutData.NextId);
                    //小さなインターバルを設ける
                    currentCut.ClearCharacter();
                    currentCut.ClearText();
                    DOVirtual.DelayedCall(INTERVAL, () => currentCut.Apply());
                    return true;
                }
            }
        }

        public void Open()
        {

        }

        public void Close()
        {

        }

        public void Dispose()
        {

        }
    }
}
