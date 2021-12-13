using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZFramework.Util;

namespace EZFramework.Game
{

    /// <summary>
    /// 利用頻度の高いゲームオブジェクトをキャッシュしておくプール
    /// </summary>
    public class GameEntityPool : SingletonMonobehaviour<GameEntityPool>
    {
        Dictionary<string, Stack<GameEntity>> pool = new Dictionary<string, Stack<GameEntity>>();

        public void Init()
        {

        }

        public override void Dispose()
        {
            foreach (Stack<GameEntity> stack in pool.Values)
            {
                foreach (GameEntity entity in stack)
                {
                    GameObject.Destroy(entity);
                }
            }
            pool.Clear();
            base.Dispose();
        }

        /// <summary>
        /// 取り出す。TODO:リストの要素を新しく確保するのを避けるため、リストは最大数をキープしてpopしなくてもいいかも
        /// </summary>
        public GameEntity Pop(string key)
        {
            if (!pool.ContainsKey(key))
            {
                //Debug.Log($"no found {key} in go pool");
                return null;
            }

            if (pool[key] == null)
                return null;

            if (pool[key].Count == 0)
                return null;

            GameEntity popped = pool[key].Pop();
            //Debug.LogError("popped ====> " + popped.gameObject.GetInstanceID() + " / count : " + pool[key].Count);
            return popped;
        }

        /// <summary>
        /// 後に使えるように取っておく。TODO:リストの要素を新しく確保するのを避けるため、リストは最大数をキープしてpopしなくてもいいかも
        /// </summary>
        public void Push(GameEntity entity)
        {

            if (pool.ContainsKey(entity.assetAddress))
            {
                pool[entity.assetAddress].Push(entity);
                //Debug.LogError("pushed ====> " + entity.gameObject.GetInstanceID() + " / count : " + pool[entity.assetAddress].Count);
            }
            else
            {
                pool.Add(entity.assetAddress, new Stack<GameEntity>());
                pool[entity.assetAddress].Push(entity);
                //Debug.LogError("pushed NEW ====> " + entity.gameObject.GetInstanceID() + " / count : " + pool[entity.assetAddress].Count);
            }
            entity.transform.position = Vector3.zero;
            entity.transform.parent = transform;
        }
    }

    /// <summary>
    /// このインターフェイスを実装するということは、リサイクル対象とすること
    /// </summary>
    public interface IReusable
    {
        /// <summary>
        /// リサイクルするための保存
        /// </summary>
        void DisposeOnReuse();

        /// <summary>
        /// リサイクルされた時の初期化処理
        /// </summary>
        void InitOnReuse();
    }
}


