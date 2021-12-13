using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace EZFramework.Game.AI
{

    /// <summary>
    /// 一つの行動。
    /// </summary>
    [Obsolete("通常のステートマシン使用時はこれを使うがノードの使用を推奨")]
    public abstract class GameEntityAIState : GameState
    {
        public event Action<GameEntityAICommand> onCommandIssued;

        /// <summary>
        /// 優先度順にソートされた遷移情報
        /// </summary>
        List<GameEntityAITransition> transitions = new List<GameEntityAITransition>();

        /// <summary>
        /// このステートからの遷移情報を登録する
        /// </summary>
        public void AddTransition(GameEntityAITransition transition)
        {
            if (!transitions.Contains(transition))
            {
                transition.fromState = this;
                transitions.Add(transition);
            }
        }

        /// <summary>
        /// 内部的に遷移が発生する条件のチェック
        /// </summary>
        public GameEntityAITransition CheckTransition() => transitions.Where(e => e.CanTransit(this)).FirstOrDefault();

        /// <summary>
        /// int値の指定により発生する条件のチェック
        /// </summary>
        public GameEntityAITransition CheckTransition(int value) => transitions.Where(e => e.CanTransit(value)).FirstOrDefault();

        /// <summary>
        /// 文字列の指定により発生する条件のチェック
        /// </summary>
        public GameEntityAITransition CheckTransition(string trigger) => transitions.Where(e => e.CanTransit(trigger)).FirstOrDefault();


        protected void SendOnCommandIssued(GameEntityAICommand command)
        {
            if (onCommandIssued != null)
                onCommandIssued(command);
        }

        //public override void OnStateActivated()
        //{

        //}
    }
}

