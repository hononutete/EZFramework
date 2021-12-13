using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public class GameEntityControllerState : GameState
    {
        protected GameEntityController owner;

        public GameEntityControllerState Init(GameEntityController owner)
        {
            this.owner = owner;
            OnInit();
            return this;
        }

        public virtual void OnInit() { }

    }
}
