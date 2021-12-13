using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace EZFramework.Game
{
    /// <summary>
    /// ノード間のステート遷移をコントロール
    /// </summary>
    public class GameNodeTree<T> where T : GameStateNode
    {
        public T rootStateNode { get; private set; }

        ///現在のアクティブな末端ノード　TODO:キャッシュは必要
        public T CurrentLeafNode
        {
            get
            {
                GameStateNode n = rootStateNode;
                while (n.ActiveChildNode != null) n = n.ActiveChildNode;
                return n as T;
            }
        }

        /// <summary>
        /// root -> 引数のノードの順番で先祖ノードのリストを返す
        /// </summary>
        public List<T> GetAncectorsFromRoot
        {
            get
            {
                List<T> list = new List<T>();
                GameStateNode n = rootStateNode;
                list.Add(n as T);
                while (n.ActiveChildNode != null) //TODO: ActiveChildNode では検索が入っている
                {
                    list.Add(n.ActiveChildNode as T);
                    n = n.ActiveChildNode;
                }
                return list;
            }
        }

        public void SelectAll(Action<T> action) => Recursive(rootStateNode, action);

        void Recursive(T parentNode, Action<T> action)
        {
            action(parentNode);
            foreach (T node in parentNode.childNodes)
            {
                if (node.IsLeafNode)
                    action(node);
                else
                    Recursive(node, action);
            }
        }

        public void SetRoot(T root) => rootStateNode = root;

        public void AddTo(int parentStateNodeId, T childStateNode)
        {
            T node = GameNodeUtil.FindById<T>(rootStateNode, parentStateNodeId);
            Debug.Assert(node != null, $"AI state not found : id = {parentStateNodeId}");

            node.AddChild(childStateNode);

            //ノードからの情報を受け取るためのコールバックを設定
        }

        public void AddToRoot(T childStateNode)
        {
            rootStateNode.AddChild(childStateNode);
        }

        public void AddTo(T parentStateNode, T childStateNode)
        {
            parentStateNode.AddChild(childStateNode);
        }

        public void RemoveNode(int stateNodeId)
        {
            T node = GameNodeUtil.FindById<T>(rootStateNode, stateNodeId);
            Debug.Assert(node != null, $"AI state not found : id = {stateNodeId}");

            if (node.parentNode.childNodes.Contains(node))
                node.parentNode.childNodes.Remove(node);
        }

        public T GetNode(int stateNodeId) => GameNodeUtil.FindById(rootStateNode, stateNodeId);

        /// <summary>
        /// 現在のステートの更新処理
        /// </summary>
        public void Update(float deltaTime) => CurrentLeafNode?.Update(deltaTime);

        public void Log()
        {
            string l = "0";

            GameStateNode n = rootStateNode;

            string path = "";
            Action<GameStateNode, string> recursive = null;
            recursive = (GameStateNode parentNode, string p) =>
            {
                int i = 0;
                foreach (GameStateNode node in parentNode.childNodes)
                {
                    if (i != 0)
                    {
                        p = p.Replace("- " + parentNode.stateId.ToString(), "             |");
                    }

                    string o;
                    if (node.IsActiveNode)
                        o = p + $"<color=red> - {node.stateId}</color>";
                    else
                        o = p + $" - {node.stateId}";

                //内部ノードは再帰処理、葉ノードが見つかったら改行
                if (node.IsInternalNode)
                        recursive(node, o);
                    else
                        l += o + "\n";
                    i++;
                }
            };

            recursive(rootStateNode, path);
            Debug.Log(l + "\n");
        }


    }
}
