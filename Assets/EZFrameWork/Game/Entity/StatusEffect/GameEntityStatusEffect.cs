using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public class GameEntityStatusEffect
    {
        public int instanceId;
        public virtual void Init() { }
        public virtual void OnAdded(GameEntity owner) { }
        public virtual void OnRemoved(GameEntity owner) { }
    }
}
