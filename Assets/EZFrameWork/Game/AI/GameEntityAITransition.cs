using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game.AI
{

    /// <summary>
    /// AIステート間の遷移を定義。木構造においては兄弟ノード間での遷移に相当する。
    /// </summary>
    public class GameEntityAITransition
    {
        /// <summary>
        /// 遷移元のノード
        /// </summary>
        public int fromNodeId;

        /// <summary>
        /// 遷移先のノード
        /// </summary>
        public int toNodeId;

        /// <summary>
        /// 遷移条件
        /// </summary>
        public GameEntityAITransitionCondition condition;

        public bool CanTransit(GameEntityAIStateNode currentState) => condition.CheckCondition(currentState);

        [Obsolete("通常のステートマシン使用時はこれを使うがノードの使用を推奨")]
        public bool CanTransit(GameEntityAIState currentState) => condition.CheckCondition(currentState);

        public bool CanTransit(int value) => condition.CheckCondition(value);
        public bool CanTransit(string trigger) => condition.CheckCondition(trigger);

        #region Deprecated
        /// <summary>
        /// 遷移元のノード
        /// </summary>
        [Obsolete("通常のステートマシン使用時はこれを使うがノードの使用を推奨")]
        public GameEntityAIState fromState;

        /// <summary>
        /// 遷移先のノード
        /// </summary>
        [Obsolete("通常のステートマシン使用時はこれを使うがノードの使用を推奨")]
        public GameEntityAIState toState;
        #endregion
    }
}
