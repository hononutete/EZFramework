using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace EZFramework.Game
{

    public static class GameEntitySpawner
    {
        static int counter = 0;

        public static async UniTask<T> InstantiateAsync<T>(string address, Transform parent = null) where T : GameEntity
        {
            //エンティティプールを検索
            T entity = GameEntityPool.Instance.Pop(address) as T;

            if (entity != null)
            {
                entity.gameObject.SetActive(true);
                entity.transform.parent = parent;
                entity.SetEntityInstanceID(counter);
                entity.InitOnReuse();
                return entity;
            }

            GameObject instance = await ResourceManager.InstantiateAsync(address, parent);
            if (instance == null)
                return null;

            T player = instance.GetComponent<T>();
            if (player == null)
                return null;

            counter++;

            player.SetEntityInstanceID(counter);
            player.Init();

            //後にプールに保存するためのアドレス
            player.assetAddress = address;

            return player;
        }

        public static async UniTask<GameEntity> InstantiateAsync(string address, Transform parent = null)
        {
            //エンティティプールを検索
            GameEntity entity = GameEntityPool.Instance.Pop(address);
            if (entity != null)
            {
                entity.gameObject.SetActive(true);
                entity.SetEntityInstanceID(counter);
                entity.InitOnReuse();
                return entity;
            }

            GameObject instance = await ResourceManager.InstantiateAsync(address, parent);
            if (instance == null)
                return null;

            GameEntity player = instance.GetComponent<GameEntity>();
            if (player == null)
                return null;

            counter++;

            player.SetEntityInstanceID(counter);
            player.Init();

            //後にプールに保存するためのアドレス
            player.assetAddress = address;

            return player;
        }


        public static void ClearCounter()
        {
            counter = 0;
        }
    }
}