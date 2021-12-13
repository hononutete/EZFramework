using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace EZFramework.Game.AI
{
    /// <summary>
    /// 木構造になっているAIステート。
    /// </summary>
    public class GameEntityAIStateNode : GameStateNode
    {
        public event Action<GameEntityAICommand> onCommandIssued;

        /// <summary>
        /// 優先度順にソートされた遷移情報。これは基本的に兄弟ノード間での遷移になる。
        /// </summary>
        List<GameEntityAITransition> transitions = new List<GameEntityAITransition>();

        /// <summary>
        /// このステートからの遷移情報を登録する
        /// </summary>
        public void AddTransition(GameEntityAITransition transition)
        {
            if (!transitions.Contains(transition))
            {
                transition.fromNodeId = stateId;
                transitions.Add(transition);
            }
        }

        /// <summary>
        /// 切り替え可能であればアクティブステートIDを切り替える。
        /// </summary>
        public bool DoTransition()
        {
            GameEntityAITransition t = transitions.Where(e => e.CanTransit(this)).FirstOrDefault();
            if (t != null) parentNode.ActiveChildNodeId = t.toNodeId;
            return t != null;
        }
        public bool DoTransition(int value)
        {
            GameEntityAITransition t = transitions.Where(e => e.CanTransit(value)).FirstOrDefault();
            if (t != null) parentNode.ActiveChildNodeId = t.toNodeId;
            return t != null;
        }
        public bool DoTransition(string trigger)
        {
            GameEntityAITransition t = transitions.Where(e => e.CanTransit(trigger)).FirstOrDefault();
            if (t != null) parentNode.ActiveChildNodeId = t.toNodeId;
            return t != null;
        }

        protected void SendOnCommandIssued(GameEntityAICommand command)
        {
            if (onCommandIssued != null)
                onCommandIssued(command);
        }


    }
}
