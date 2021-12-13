using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EZFramework.Game
{

    //TODO: インデックスでアクセスできるようにして高速化
    public class GameStateNode : GameState
    {

        public GameStateNode parentNode;
        public List<GameStateNode> childNodes = new List<GameStateNode>();
        public int depth => parentNode == null ? 0 : parentNode.depth + 1;
        public bool IsActiveNode => parentNode == null ? true : parentNode.ActiveChildNodeId == stateId;
        public bool IsRootNode => parentNode == null;
        /// <summary>
        /// 内部ノードがどうか
        /// </summary>
        public bool IsInternalNode => childNodes.Count != 0;

        public bool IsLeafNode => childNodes.Count == 0;

        public void AddChild(GameStateNode node)
        {
            if (!childNodes.Contains(node))
            {
                childNodes.Add(node);
                node.parentNode = this;
            }
        }

        #region Node Activation


        /// <summary>
        /// 子ノードを持つ場合、子ノードのデフォルトノート
        /// </summary>
        public int defaultChildNodeId;

        /// <summary>
        /// 現在アクティブなノードID
        /// </summary>
        public int _activeChildNodeId;
        public int ActiveChildNodeId
        {
            get { return _activeChildNodeId; }
            set
            {
                ActiveChildNode?.OnStateDeactivated();
                _activeChildNodeId = value;
                ActiveChildNode?.OnStateActivated();
            }
        }

        /// <summary>
        /// 現在アクティブなノード
        /// </summary>
        public GameStateNode ActiveChildNode => childNodes.FirstOrDefault(e => e.stateId == ActiveChildNodeId);

        public override void OnStateActivated()
        {
            base.OnStateActivated();

            //このステートがアクティブになった時に、内部ノードだった場合
            if (IsInternalNode)
            {
                //最初のステートはデフォルトステート
                ActiveChildNodeId = defaultChildNodeId;

                //再起的に子ノードのアクティブ化
                //ActiveChildNode?.OnStateActivated();
            }
        }

        public override void OnStateDeactivated()
        {
            //このステートが非アクティブになった時に、内部ノードだった場合
            if (IsInternalNode)
            {
                //デフォルトステートを現在のステートに設定しておく
                defaultChildNodeId = ActiveChildNodeId;

                //再起的に子ノードのアクティブ化
                ActiveChildNode?.OnStateDeactivated();
            }
        }

        #endregion
    }
}





