using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


namespace EZFramework.Game
{
    public class GameEntityStatusEffectManager<T> where T : GameEntityStatusEffect
    {
        public const int INVALID_ID = -1;
        int idCounter = 0;
        public GameEntity owner;

        public GameEntityStatusEffectManager(GameEntity owner)
        {
            this.owner = owner;
        }

        List<T> statusEffects = new List<T>();

        /// <summary>
        /// 状態効果を追加する。同一ステータスマネージャー内での識別用IDを返す。
        /// </summary>
        public int Add(T statusEffect)
        {
            if (!statusEffects.Contains(statusEffect))
            {
                idCounter++;
                statusEffects.Add(statusEffect);
                statusEffect.OnAdded(owner);
                statusEffect.instanceId = idCounter;
                return idCounter;
            }
            return INVALID_ID;
        }

        public void Remove(T statusEffect)
        {
            if (statusEffects.Contains(statusEffect))
            {
                statusEffects.Remove(statusEffect);
                statusEffect.OnRemoved(owner);
            }
        }

        public void RemoveByInstanceId(int id)
        {
            T t = statusEffects.FirstOrDefault(e => e.instanceId == id);
            if (t != null)
            {
                statusEffects.Remove(t);
                t.OnRemoved(owner);
            }
        }

        public List<T> Find(Predicate<T> condition) => statusEffects.Where(e => condition(e)).ToList();
    }
}