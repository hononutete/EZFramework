using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    /// <summary>
    /// プレイヤーコントローラをステートマシンで管理する場合、コントローラーの状態（ステート）を表す一つのブロックを表すクラス
    /// </summary>
    public class GameState
    {
        public float elapsedTime { get; private set; }

        public int stateId;

        public virtual void Update(float deltaTime)
        {
            elapsedTime += deltaTime;
        }

        public virtual void OnStateActivated()
        {
            elapsedTime = 0;
        }

        public virtual void OnStateDeactivated()
        {
            elapsedTime = 0;
        }

        public virtual void Reset() { }

        public virtual void Dispose() { }
    }
}


