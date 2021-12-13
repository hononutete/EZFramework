using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game.AI
{
    public class GameEntityAITransitionConditionInteger : GameEntityAITransitionCondition
    {
        public GameEntityAITransitionConditionInteger(EComparision comparision, int threshold)
        {
            this.comparision = comparision;
            this.threshold = threshold;
        }

        EComparision comparision;
        int threshold;

        /// <summary>
        ///  条件を達成しているかどうかの判定
        /// </summary>
        public override bool CheckCondition(int value) => Examin(value);

        /// <summary>
        /// 達成しているかどうかのテストを実行。
        /// </summary>
        protected bool Examin(int value)
        {
            if (comparision == EComparision.LESS)
            {
                return value < threshold;
            }
            else if (comparision == EComparision.GREATER)
            {
                return value > threshold;
            }
            else if (comparision == EComparision.LEQUAL)
            {
                return value <= threshold;
            }
            else if (comparision == EComparision.GEQUAL)
            {
                return value >= threshold;
            }
            else if (comparision == EComparision.EQUAL)
            {
                return value == threshold;
            }
            else if (comparision == EComparision.NOT_EQUAL)
            {
                return value != threshold;
            }
            else if (comparision == EComparision.ALWAYS)
            {
                return true;
            }
            return false;
        }
    }
}