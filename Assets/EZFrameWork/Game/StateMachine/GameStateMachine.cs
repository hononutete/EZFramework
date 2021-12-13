using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public class GameStateMachine<T> where T : GameState
    {

        Dictionary<int, T> stateMachine = new Dictionary<int, T>();
        int currentState = INVALID;
        public const int INVALID = -1;

        public int StateCount => stateMachine.Count;

        public void AddState(int id, T state)
        {
            state.stateId = id;

            if (!stateMachine.ContainsKey(id))
                stateMachine.Add(id, state);
        }

        public void RemoveState(int id)
        {
            if (stateMachine.ContainsKey(id))
                stateMachine.Remove(id);
        }

        public virtual void Dispose()
        {
            foreach (GameState state in stateMachine.Values)
                state.Dispose();
            stateMachine.Clear();
        }

        public virtual void Reset()
        {
            foreach (T state in stateMachine.Values)
                state.Reset();
        }

        public T CurrentState => currentState == INVALID ? null : stateMachine[currentState];

        public T GetState(int stateId) => stateMachine.ContainsKey(stateId) ? stateMachine[stateId] : null;

        /// <summary>
        /// 現在のステートを遷移させる
        /// </summary>
        public T ChangeState(int to)
        {
            //現在のステートの終了処理
            if (currentState != INVALID)
            {
                if (stateMachine.ContainsKey(currentState))
                {
                    stateMachine[currentState].OnStateDeactivated();
                }
            }

            //次のステートの開始処理
            if (stateMachine.ContainsKey(to))
            {
                currentState = to;
                stateMachine[to].OnStateActivated();
            }
            return stateMachine[currentState];
        }

        /// <summary>
        /// 現在のステートの更新処理
        /// </summary>
        public void Update(float deltaTime) => CurrentState?.Update(deltaTime);

        public void Log(string header = "")
        {
            string l = header + "\n";
            l += "state count = " + stateMachine.Count;
            foreach (KeyValuePair<int, T> pair in stateMachine)
            {
                l += pair.Key + ", " + pair.Value + "\n";
            }
            Debug.Log(l);
        }
    }
}