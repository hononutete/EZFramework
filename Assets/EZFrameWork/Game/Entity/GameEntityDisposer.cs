using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EZFramework.Game
{
    public static class GameEntityDisposer
    {
        public static void Dispose(GameEntity entity)
        {
            if (entity.doRecycle)
            {
                entity.DisposeOnReuse();
                entity.gameObject.SetActive(false);
                GameEntityPool.Instance.Push(entity);
            }
            else
            {
                entity.Destroy();
                UnityEngine.Object.Destroy(entity.gameObject);
            }

        }

        public static void DisposeInTime(GameEntity entity, float time)
        {
            DOVirtual.DelayedCall(time, () => Dispose(entity));
        }
    }
}