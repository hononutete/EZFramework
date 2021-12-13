using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EZFramework.Realtime;

namespace EZFramework.Game
{

    public class GameEntity : MonoBehaviour, IReusable
    {
        #region RealtimeNetwork

        public ushort ownerSessionId;
        public ushort networkInstanceId;
        public bool isOwner = false;
        public bool doSyncPosition = false;
        public bool doSyncRotation = false;
        public float syncFrequency = 0.01f;
        float frequecyCounter = 0;

        bool isSyncPosInitialized = false;
        (Vector2 position, float time) lastSyncPosition;
        //Vector3 lastSyncRotation;
        Vector2 dDir = Vector2.zero;
        float deltaTime = 0;
        float deltaTimeCount = 0;

        public bool IsDisposed { get; private set; }

        public virtual void OnInstantiatedOnNetwork() { }

        public virtual void SetTransform(Vector2 position)
        {
            //現在位置は予測地点を下にもしかしたら間違っているかもしれない

            if (!isSyncPosInitialized)
            {
                InitLasySyncPosition(position);
                return;
            }

            //最後の位置と、受け取った現在地から、予測目的地を計算
            Vector2 dir = position - transform.position.Vector2XY();
            float deltaTime = Time.time - lastSyncPosition.time;
            //Debug.LogError($"dir = {dir}, sent position = {position}, myPos = {transform.position.Vector2XY()}, deltaTime = {deltaTime}");

            //このまま進んだと仮定して到達する地点を目標とする
            dDir = dir / deltaTime;
            this.deltaTime = deltaTime;
            deltaTimeCount = 0;

            //destination = new Vector3(position.x, position.y, transform.position.z);
            //transform.position = new Vector3(position.x, position.y, transform.position.z);

            lastSyncPosition.position = position;
            lastSyncPosition.time = Time.time;

        }

        void InitLasySyncPosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            lastSyncPosition.position = position;
            lastSyncPosition.time = Time.time;
            isSyncPosInitialized = true;
            deltaTime = 0;
            deltaTimeCount = 0;
        }

        public virtual void SetRotation(Vector3 eulerAngles)
        {
            transform.eulerAngles = eulerAngles;
        }

        public virtual void SendSyncTransform() => RTNetworkClient.Instance.SendGameEvent(GameEventCode.TRANSFORM_POSITION_SYNC, networkInstanceId, transform.position.Vector2XY());
        public virtual void SendSyncRotation() => RTNetworkClient.Instance.SendGameEvent(GameEventCode.TRANSFORM_ROTATION_SYNC, networkInstanceId, transform.eulerAngles);

        public void BroadcastRPC(string funcName, object[] parameters)
        {
            SendMessage(funcName, parameters);
        }

        public void BroadcastPropertySync(string propertyName, object value)
        {
            SendMessage(propertyName, value);

        }

        #endregion

        public int EntityInstanceId { get; private set; }
        public void SetEntityInstanceID(int id) => EntityInstanceId = id;

        public GameEntityController Controller { get; private set; }

        public virtual void Update()
        {
            if (isOwner)
            {
                if (frequecyCounter >= 1.0f / RTNetworkClient.Instance.NetworkFrameRate)
                {
                    if (doSyncPosition)
                        SendSyncTransform();
                    if (doSyncRotation)
                        SendSyncRotation();

                    frequecyCounter = 0;
                }
                else
                    frequecyCounter += Time.deltaTime;
            }
            else
            {
                //受信した位置情報の補完処理
                if (doSyncPosition)
                {
                    if (deltaTimeCount < deltaTime)
                    {
                        Vector2 d = dDir * Time.deltaTime;
                        transform.position += new Vector3(d.x, d.y, 0);
                        deltaTimeCount += Time.deltaTime;
                    }
                }
            }
        }

        public virtual void Init()
        {
            isSyncPosInitialized = false;
            _isReused = false;
            IsDisposed = false;
            deltaTime = 0;
            deltaTimeCount = 0;
        }

        /// <summary>
        /// 破棄時に呼ばれる。リサイクルされるときはDiposeOnReuseも呼ばれる
        /// </summary>
        public virtual void Destroy()
        {
            if (onDestroy != null)
                onDestroy();

            Destroy(this.gameObject);
        }

        public void SetController(GameEntityController controller)
        {
            Controller = controller;
        }

        protected virtual void OnControllerSet(GameEntityController controller) { }

        public float SqrDistance(GameEntity entity) => (entity.transform.position - transform.position).sqrMagnitude;

        #region Reusable

        public bool doRecycle = true;
        public string assetAddress { get; set; }

        bool _isReused = false;
        public bool IsReused => _isReused;

        public event Action onDipsoseOnResuse;
        public event Action onInitOnResuse;
        public event Action onDestroy;

        /// <summary>
        /// リサイクルするための保存
        /// </summary>
        public void DisposeOnReuse()
        {
            if (onDipsoseOnResuse != null)
                onDipsoseOnResuse();

            //継承先でBaseを書き忘れないために空の仮想関数を用意
            OnDisposeOnReuse();

            ownerSessionId = 0;
            networkInstanceId = 0;
            isOwner = false;
            doSyncPosition = false;
            doSyncRotation = false;
            frequecyCounter = 0;
            isSyncPosInitialized = false;
            lastSyncPosition = (Vector2.zero, 0);
            dDir = Vector2.zero;
            deltaTime = 0;
            deltaTimeCount = 0;
            IsDisposed = true;
        }

        /// <summary>
        /// ReuseのDisposeが呼ばれた時に呼ばれる処理
        /// </summary>
        protected virtual void OnDisposeOnReuse() { }

        /// <summary>
        /// リサイクルされた時の初期化処理
        /// </summary>
        public virtual void InitOnReuse()
        {
            _isReused = true;
            IsDisposed = false;
            if (onInitOnResuse != null)
                onInitOnResuse();
        }

        protected IEnumerator DisposeInTime(float time)
        {
            yield return new WaitForSeconds(time);
            GameEntityDisposer.Dispose(this);
        }

        #endregion
    }
}
