using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game.AI
{
    /// <summary>
    /// 遷移条件を指定
    /// </summary>
    public class GameEntityAITransitionCondition
    {
        /// <summary>
        /// 同時に条件が発生した時の優先度。高い値の方が優先順位が高い
        /// </summary>
        public int layer;

        /// <summary>
        ///  条件を達成しているかどうかの判定
        /// </summary>
        public virtual bool CheckCondition(GameEntityAIStateNode currentState) => false;

        [Obsolete("通常のステートマシン使用時はこれを使うがノードの使用を推奨")]
        public virtual bool CheckCondition(GameEntityAIState currentState) => false;

        public virtual bool CheckCondition(int value) => false;
        public virtual bool CheckCondition(string trigger) => false;

    }
}
