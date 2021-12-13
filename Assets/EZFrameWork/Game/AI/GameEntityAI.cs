using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game.AI
{

    public class GameEntityAI
    {
        /// <summary>
        /// アップデート頻度を秒で指定
        /// </summary>
        public float frequency = 0.1f;
        public float counter = 0;

        GameNodeTree<GameEntityAIStateNode> nodeTree;
        GameEntity owner;
        int defaultStateId;
        public event Action<GameEntityAICommand> onCommandIssued;

        //遷移テーブル、idで指定
        //必要な情報

        public GameEntityAI(GameEntity owner)
        {
            this.owner = owner;
        }

        public GameEntityAIStateNode GetCurrentStateNode() => nodeTree.CurrentLeafNode;

        public void SetNodeTree(GameNodeTree<GameEntityAIStateNode> nodeTree)
        {
            this.nodeTree = nodeTree;

            //ノードの全てのノードに対してコマンドイベントを取得 //TODO: 動的なノードの追加はサポートされていない
            nodeTree.SelectAll(node => node.onCommandIssued += OnCommandIssued);
        }

        /// <summary>
        /// 初回アクティブ化
        /// </summary>
        public void StartDefaultState() => nodeTree.rootStateNode.OnStateActivated();

        //TODO: ステートマシン内のステートからイベントを削除する。
        public void Dispose() { }

        public void Reset() { }

        void OnCommandIssued(GameEntityAICommand command)
        {
            if (onCommandIssued != null)
                onCommandIssued(command);
        }

        /// <summary>
        /// トリガー指定が遷移条件になる場合
        /// </summary>
        public void CheckInt(int value)
        {
            List<GameEntityAIStateNode> ancestors = nodeTree.GetAncectorsFromRoot;
            foreach (GameEntityAIStateNode n in ancestors)
            {
                if (n.DoTransition())
                    break;
            }
        }

        /// <summary>
        /// トリガー指定が遷移条件になる場合
        /// </summary>
        public void CheckTrigger(string trigger)
        {
            List<GameEntityAIStateNode> ancestors = nodeTree.GetAncectorsFromRoot;
            foreach (GameEntityAIStateNode n in ancestors)
            {
                if (n.DoTransition())
                    break;
            }
        }

        public void Update()
        {
            if (counter < frequency)
            {
                counter += Time.deltaTime;
                return;
            }

            counter = 0;

            //末端のアクティブノードのアップデート
            nodeTree.Update(frequency);

            //ステートの遷移条件をチェックし、発見した場合、遷移処理を実行
            List<GameEntityAIStateNode> ancestors = nodeTree.GetAncectorsFromRoot;
            foreach (GameEntityAIStateNode n in ancestors)
            {
                if (n.DoTransition())
                    break;
            }
        }
    }
}