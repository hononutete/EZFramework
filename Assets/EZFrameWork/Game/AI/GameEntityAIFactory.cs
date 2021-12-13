using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace EZFramework.Game.AI
{
    public class MAIState
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        /// <summary>
        /// StateType
        /// </summary>
        public int StateType;
        /// <summary>
        /// ParentStateID
        /// </summary>
        public int ParentStateID;
        /// <summary>
        /// DefaultStateID
        /// </summary>
        public int DefaultStateID;
    }

    public class MAIStateTransition 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        /// <summary>
        /// TemplateID
        /// </summary>
        public int TemplateID;
        /// <summary>
        /// FromStateID
        /// </summary>
        public int FromStateID;
        /// <summary>
        /// ToStateID
        /// </summary>
        public int ToStateID;
    }

    public class MAIStateTransitionTemplate 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        /// <summary>
        /// ConditionType
        /// </summary>
        public int ConditionType;
        /// <summary>
        /// Trigger
        /// </summary>
        public string Trigger;
        /// <summary>
        /// ComparisionType
        /// </summary>
        public int ComparisionType;
        /// <summary>
        /// TargetParameterType
        /// </summary>
        public int TargetParameterType;
        /// <summary>
        /// Threshold
        /// </summary>
        public float Threshold;
    }

    public static class AIMasterData
    {
        public static Dictionary<int, MAIState> MAIStateDic;
        public static Dictionary<int, List<MAIState>> MAIStateByParentId;
        public static Dictionary<int, List<MAIStateTransition>> MAIStateTransitionByFromStateId;
        public static Dictionary<int, MAIStateTransitionTemplate> MAIStateTransitionTemplateDic;

    }

    public static class GameEntityAIFactory
    {
        /// <summary>
        /// 一般的な、攻撃できるところまで行って、攻撃するAI
        /// </summary>
        public static GameEntityAI Load(GameEntity owner, int mAIStateId)
        {
            GameEntityAI ai = new GameEntityAI(owner);

            MAIState mAIState = AIMasterData.MAIStateDic[mAIStateId];

            //*** ノードの木構造作成 ***
            GameNodeTree<GameEntityAIStateNode> nodeTree = new GameNodeTree<GameEntityAIStateNode>();

            //ルートステート
            GameEntityAIStateNode root = new GameEntityAIStateNode();
            root.stateId = mAIState.ID; //0がルート
            root.defaultChildNodeId = mAIState.DefaultStateID; //子ノードがある場合、デフォルトIDを設定

            // 再起的にツリー内のノード作成
            RecursiveAdd(owner, root);

            //全てのステートを登録
            nodeTree.SetRoot(root);

            ai.SetNodeTree(nodeTree);

            return ai;
        }

        /// <summary>
        /// 再起的にツリー内のノード作成
        /// </summary>
        static void RecursiveAdd(GameEntity owner, GameEntityAIStateNode parentNode)
        {
            //ルートが親に指定されているデータを検索
            List<MAIState> childList = AIMasterData.MAIStateByParentId[parentNode.stateId];

            foreach (MAIState child in childList)
            {
                //ステート作成
                GameEntityAIStateNode node = CreateState(owner, child.ID);

                //ステートの遷移情報を作成し、ノードの追加
                CreateTransition(owner, child.ID).ForEach(e => node.AddTransition(e));

                //親ステートに足す
                parentNode.AddChild(node);

                //keyがある場合、そのchildが、さらに他のステートの親であるということ
                if (AIMasterData.MAIStateByParentId.ContainsKey(child.ID))
                    RecursiveAdd(owner, node);
            }
        }

        //TODO:reflectionを考慮
        static GameEntityAIStateNode CreateState(GameEntity owner, int stateId)
        {
            MAIState state = AIMasterData.MAIStateDic[stateId];
            GameEntityAIStateNode node = null;

            //reflectionを使ったクラス生成
            string className = Enum.GetName(typeof(EAIStateType), state.StateType);
            Type type = Type.GetType(className);
            node = Activator.CreateInstance(type, owner) as GameEntityAIStateNode;
            node.stateId = stateId;
            node.defaultChildNodeId = state.DefaultStateID;
            return node;
        }

        static List<GameEntityAITransition> CreateTransition(GameEntity owner, int fromStateId)
        {
            if (!AIMasterData.MAIStateTransitionByFromStateId.ContainsKey(fromStateId))
                return new List<GameEntityAITransition>();

            List<MAIStateTransition> transitions = AIMasterData.MAIStateTransitionByFromStateId[fromStateId];
            return transitions.Select(e =>
           {
               GameEntityAITransition t = new GameEntityAITransition();
               t.fromNodeId = e.FromStateID;
               t.toNodeId = e.ToStateID;

               MAIStateTransitionTemplate template = AIMasterData.MAIStateTransitionTemplateDic[e.TemplateID];

           //条件を生成
           switch ((EAIStateTransitionConditionType)template.ConditionType)
               {
                   case EAIStateTransitionConditionType.TRIGGER:
                       t.condition = new GameEntityAITransitionConditionTrigger(template.Trigger);
                       break;
                   case EAIStateTransitionConditionType.INTEGER:
                   //t.condition = new GameEntityAITransitionConditionInteger(e.); //TODO: 未実装
                   break;
                   case EAIStateTransitionConditionType.BY_MASTER:
                       EComparision comparision = (EComparision)template.ComparisionType;

                   //対象パラメーターによって変わる
                   //reflectionを使ったクラス生成
                   string className = Enum.GetName(typeof(EAIStateTransitionTargetParameterType), template.TargetParameterType);
                       Type type = Type.GetType(className);
                       t.condition = Activator.CreateInstance(type, owner, comparision, Mathf.FloorToInt(template.Threshold)) as GameEntityAITransitionCondition;

                   //対象パラメーターによって変わる

                   break;
                   default:
                       break;
               }
               return t;
           }).ToList();
        }

    }
}