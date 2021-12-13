using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EZFramework.Game;

namespace EZFramework.Realtime
{

    public class GameNetworkObjectSpawner
    {
        public static async Task<GameEntity> SpawnAsync(string rcName, ushort ownerSessionId, ushort networkInstanceId, bool isOwner, Vector3 position = default(Vector3))
        {
            GameEntity instance = await GameEntitySpawner.InstantiateAsync(rcName, null);

            if (instance != null)
            {
                instance.transform.position = position;
                instance.ownerSessionId = ownerSessionId;
                instance.networkInstanceId = networkInstanceId;
                instance.isOwner = isOwner;
                instance.OnInstantiatedOnNetwork();
            }
            return instance;
        }
    }

}