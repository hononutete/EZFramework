using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public static class GameNodeUtil
    {

        public static T FindById<T>(T rootNode, int id) where T : GameStateNode => RecursiveFind<T>(rootNode, id);

        static T RecursiveFind<T>(T parentNode, int id) where T : GameStateNode
        {
            //同じidだったら返す
            if (parentNode.stateId == id)
                return parentNode;

            foreach (GameStateNode node in parentNode.childNodes)
            {
                GameStateNode result = RecursiveFind(node, id);
                if (result != null)
                    return result as T;
            }

            //最下層だが、見つからなかった。nullを返す
            return null;
        }

        /// <summary>
        /// root -> 引数のノードの順番で先祖ノードのリストを返す
        /// </summary>
        public static List<T> GetAncectorsToRoot<T>(T node) where T : GameStateNode
        {
            List<T> ancestors = new List<T>();
            T currentNode = node;
            while (currentNode.parentNode != null)
                ancestors.Add(currentNode.parentNode as T);

            return ancestors;
        }
    }
}


