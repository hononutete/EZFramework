using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

namespace EZFramework.Game
{
    public class GameEntityMove : MonoBehaviour
    {
        public enum EMoveMode
        {
            NONE, USER, WAYPOINT, PATH
        }
        public EMoveMode MoveMode { get; private set; } = EMoveMode.USER;

        public CinemachineDollyCart dollyCart;
        public MsgHandler onPathFinished = null;
        public float pathMoveSpeed = 1.0f;
        public bool IsPathRunngin => dollyCart.m_Path != null;

        protected GameEntity owner;
        public Vector3 direction { get; private set; } = Vector3.zero;

        public Vector3 velocity { get; private set; }
        public Vector3 lastFramePosition { get; private set; }

        public float moveSpeed = 0.0f;

        float completedPathDistance = 0;
        float onGoingPathDistance = 0;


        public virtual void Init(GameEntity owner)
        {
            this.owner = owner;
            dollyCart = GetComponent<CinemachineDollyCart>();
            completedPathDistance = 0;
            onGoingPathDistance = 0;
        }

        public virtual void Reset()
        {
            moveSpeed = 0.0f;
            completedPathDistance = 0;
            onGoingPathDistance = 0;
            direction = Vector3.zero;
            RemovePath();
        }

        // Update is called once per frame
        void Update()
        {
            //移動方向更新
            if (MoveMode == EMoveMode.WAYPOINT)
            {
                UpdateWayPoint();

                //目的地についてかどうか確認
                if (MoveMode == EMoveMode.WAYPOINT)
                    CheckReachDestination();
            }
            //パス上では位置と回転もパスに制御される
            else if (MoveMode == EMoveMode.PATH)
            {
                UpdatePath();
            }
            else if (MoveMode == EMoveMode.USER)
            {
                UpdateUser();
            }

            //速度を算出
            velocity = (transform.position - lastFramePosition) / Time.deltaTime;
            lastFramePosition = transform.position;
        }

        public virtual void SetDirection(Vector3 v)
        {
            if (MoveMode == EMoveMode.PATH)
                RemovePath();

            direction = v.normalized;
        }

        public virtual void UpdateUser()
        {

        }

        public void SetMoveActive(bool isActive) => MoveMode = isActive ? EMoveMode.USER : EMoveMode.NONE;

        #region Waypoint

        Vector3 destinationPoint;
        MsgHandler waypointAwaiter = new MsgHandler();

        public async Task SetDestination(Vector3 destinationPoint)
        {
            MoveMode = EMoveMode.WAYPOINT;
            //TODO:現状は単純な移動のみ、将来的にはnavmeshを使うパターンも検討
            this.destinationPoint = destinationPoint;
            await waypointAwaiter;
        }

        protected virtual void UpdateWayPoint()
        {
            direction = (destinationPoint - transform.position).normalized * moveSpeed;
        }

        const float WAYPOINT_TOUCH_THRESHOLD = 0.01f;
        void CheckReachDestination()
        {
            if (Vector3.SqrMagnitude(destinationPoint - transform.position) < WAYPOINT_TOUCH_THRESHOLD)
            {
                MoveMode = EMoveMode.USER;
                direction = Vector3.zero;
                waypointAwaiter.Handle();
                waypointAwaiter.Reset();
            }
        }

        #endregion

        #region Path

        public void SetCinemachinePath(CinemachinePathBase path, float speed = 0, Action onFinished = null)
        {
            dollyCart.m_Path = path;
            dollyCart.m_Position = 0;
            dollyCart.m_Speed = speed;

            //動きはパスに委ねる
            MoveMode = EMoveMode.PATH;
            //isActive = false;

            onPathFinished = MsgHandler.Create();

            if (onFinished != null)
            {
                //TODO:こっちは削除予定
                //onPathFinishedArg = onFinished;
                onPathFinished.AddEvent(onFinished);
            }
        }

        void UpdatePath()
        {
            //パス上に乗っている時
            if (dollyCart.m_Path != null)
            {
                //パス終了時
                if (dollyCart.m_PositionUnits == CinemachinePathBase.PositionUnits.Normalized)
                {
                    if (dollyCart.m_Position >= 1.0f)
                    {
                        completedPathDistance += dollyCart.m_Path.PathLength;
                        onPathFinished?.Handle(); //注意：ここで次のSetCinemachinePathが呼ばれる、つまり下の行でonPathFinished = nullとかするとバグになる
                                                  //onPathFinished = null;
                    }
                    //パスの途中
                    else
                    {
                        onGoingPathDistance = dollyCart.m_Position;
                        //dollyCart.m_Speed += OPRAppManager.Instance.CurrentGameMode.AccelerationRate
                        //    * (OPRAppManager.Instance.CurrentGameMode.MaxRunSpeed - dollyCart.m_Speed)
                        //    * Time.deltaTime;
                    }
                }
                else if (dollyCart.m_PositionUnits == CinemachinePathBase.PositionUnits.Distance)
                {
                    if (dollyCart.m_Position >= dollyCart.m_Path.PathLength)
                    {
                        completedPathDistance += dollyCart.m_Path.PathLength;
                        onPathFinished?.Handle(); //注意：ここで次のSetCinemachinePathが呼ばれる、つまり下の行でonPathFinished = nullとかするとバグになる
                                                  //onPathFinished = null;
                    }
                    //パスの途中
                    else
                    {
                        onGoingPathDistance = dollyCart.m_Position;
                        //dollyCart.m_Speed += OPRAppManager.Instance.CurrentGameMode.AccelerationRate
                        //    * (OPRAppManager.Instance.CurrentGameMode.MaxRunSpeed - dollyCart.m_Speed)
                        //    * Time.deltaTime;
                    }
                }
            }
        }

        public void RemovePath()
        {
            if (dollyCart != null)
            {
                dollyCart.m_Path = null;
                dollyCart.m_Speed = 0;
                dollyCart.m_Position = 0;
            }
            MoveMode = EMoveMode.USER;
            onPathFinished?.Reset();
            onPathFinished = null;
        }

        public void ClearePathMovedDistance()
        {
            completedPathDistance = 0;
            onGoingPathDistance = 0;
        }

        public float GetPathMovedDistance()
        {
            return completedPathDistance + onGoingPathDistance;
        }


        #endregion
    }
}