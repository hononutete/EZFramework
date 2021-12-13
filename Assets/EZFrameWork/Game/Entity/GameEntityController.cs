using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EZFramework.Game.AI;

namespace EZFramework.Game
{

    public enum EEntityControllerState
    {
        NEUTRAL, ACTION, MOVE_GRID, MOVE, MOVE_DIRECTION, ATTACK, PLAY_CARD, GAMEOVER
    }

    public class GameEntityController : MonoBehaviour, IDisposable

    {
        /// <summary>
        /// ローカルクライアントが操作する唯一のプレイヤーコントローラーをセットして使う用
        /// </summary>
        protected static GameEntityController _main = null;
        public static GameEntityController main
        {
            get
            {
                if (_main == null)
                {
                    GameObject go = GameObject.Find("GameEntityController");
                    if (go == null)
                    {
                        go = new GameObject("GameEntityController");
                        _main = go.AddComponent<GameEntityController>(); //TODO:共通でない
                    }
                    else
                    {
                        _main = go.GetComponent<GameEntityController>();
                    }
                    //_main.Init(); //TODO: Initは手動で！
                }
                return _main;
            }
        }

        GameEntity _target;

        /// <summary>
        /// コントローラーの操作対象となるエンティティプレイヤー
        /// </summary>
        public GameEntity Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                if (_target != null)
                {
                    OnTargetSet();
                    _target.SetController(this);
                }
            }
        }

        public static void CreateMain()
        {
            //事前に取得 　
            GameEntityController tmp = main;
        }

        public static void Destruct()
        {
            _main = null;
        }

        public virtual void Dispose()
        {
            controllerStateMachine.Dispose();

            AI?.Dispose();
        }

        public virtual void Reset()
        {
            controllerStateMachine.Reset();

            AI?.Reset();
        }

        protected virtual void OnTargetSet() { }


        #region State Machine

        /// <summary>
        /// ステートマシン
        /// </summary>
        protected GameStateMachine<GameState> controllerStateMachine = new GameStateMachine<GameState>();

        /// <summary>
        /// プレイヤーコントローラーのステートが変更された時に呼ばれる。切り替わる直前に呼ばれる。引数は次のステート
        /// </summary>
        public event Action<EEntityControllerState> onEntityStateChanged;

        ///// <summary>
        ///// The state of the  e player controller.
        ///// </summary>
        //EEntityControllerState _eEntityControllerState;

        ///// <summary>
        ///// プレイヤーコントローラーの現在のステート
        ///// </summary>
        //public EEntityControllerState EntityControllerState
        //{
        //    get { return _eEntityControllerState; }
        //    set
        //    {
        //        _eEntityControllerState = value;
        //        controllerStateMachine.ChangeState((int)value);
        //    }
        //}

        int _eEntityControllerState;

        public int EntityControllerState
        {
            get { return _eEntityControllerState; }
            set
            {
                _eEntityControllerState = value;
                controllerStateMachine.ChangeState(value);
            }
        }

        public GameState CurrentControllerState
        {
            get { return controllerStateMachine?.CurrentState; }
        }

        public GameState GetControllerState(int controllerStateId) => controllerStateMachine.GetState(controllerStateId);

        //public virtual void Init() { }
        public virtual void AddStates() { }




        //   void Update()
        //{
        //       CurrentControllerState?.Update(Time.deltaTime);
        //}
        #endregion

        #region AI

        public GameEntityAI AI { get; private set; }
        protected GameEntityAICommand command;

        public void SetAI(GameEntityAI ai)
        {
            //TODO:同じキャラを使いまわすのであれば、aiを作り直す必要がない。AIにはキャラの参照を保持してある。もしもキャラが変わるのであればその上書きが必要
            AI = ai;//GameEntityAIFactory.Load(Target.Cast<GameEntity>(), mAIStateId);
        }

        public void StartAI()
        {
            if (AI != null)
            {
                AI.onCommandIssued += OnCommandIssed;
                AI.StartDefaultState();
            }
        }

        /// <summary>
        /// AIのステート遷移のためのトリガーをセット
        /// </summary>
        public void SetAITrigger(string trigger)
        {
            AI?.CheckTrigger(trigger);
        }

        protected virtual void OnCommandIssed(GameEntityAICommand command) { }

        #endregion

        public virtual void Update()
        {
            AI?.Update();

            //コントローラーステートの更新処理
            controllerStateMachine.Update(Time.deltaTime);
        }

        public virtual void InitOnReuse()
        {
            //TODO: AIのステートを初期化する必要があるか
        }
    }
}