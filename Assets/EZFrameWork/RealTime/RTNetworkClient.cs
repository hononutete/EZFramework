using System.Collections;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EZFramework.Util;
using EZFramework.Game;

namespace EZFramework.Realtime
{

    public enum ERegion
    {
        JP,
        CN,
        Asia,
        Eu,
        US
    }

    public enum GameEventCode
    {
        MASTER_CLIENT = 1,
        GAME_START = 2,
        GAMEOBJECT_INSTANCING = 3,
        TRANSFORM_POSITION_SYNC = 4,
        GAMEOBJECT_DESTROY = 5,
        ALL_DROPS = 6,
        PICK_DROP = 7,
        SET_DROP_OPERATOR = 8,
        CHANGE_DROP = 9,
        RPC = 10,
        TRANSFORM_ROTATION_SYNC = 11,
        PROP_SYNC = 12,
        SYNC_MSG = 13,
        UPDATE_NPC_AI = 14,
        PROJECTILE_PARAMS = 15,
        ADD_ZONE_DURATION = 16,
        CHANGE_ZONE_LEVEL = 17,
        IMPACT_ATTACK = 18,
    }

    public static class PropertyTypeCode
    {
        public const ushort INT = 1;
        public const ushort FLOAT = 2;
        public const ushort STRING = 3;
        public const ushort VECTOR2 = 4;
        public const ushort VECTOR3 = 5;
        public const ushort VECTOR4 = 6;
        public const ushort BOOL = 7;
        public const ushort USHORT = 8;
        public const ushort BYTE = 9;
    }

    public static class SyncMsgCode
    {
        public const ushort ALL_PLAYER_SPAWNED = 1;
        public const ushort ALL_BOARD_SPAWNED = 2;
    }

    public enum ERTNetworkClientMode
    {
        NONE, ONLINE, OFFLINE
    }

    public class RTNetworkClient : SingletonMonobehaviour<RTNetworkClient>
    {
        struct PropSyncField
        {
            public string propertyName;
            public object value;
            public float timeStamp;
        }

        IRTNetworkClient client;

        ushort increment = 0;

        bool _isMasterClient = false;
        public bool IsMasterClient
        {
            get
            {
                if (ClientMode == ERTNetworkClientMode.OFFLINE)
                    return true;
                else
                    return _isMasterClient;
            }
        }

        public ERTNetworkClientMode ClientMode
        {
            get
            {
                if (client is LocalRTNetworkClient)
                    return ERTNetworkClientMode.OFFLINE;
                else
                    return ERTNetworkClientMode.ONLINE;
            }
        }

        public ERegion region = ERegion.JP;
        public string appId = "";


        public event Action onCreateRoomFailed;
        public event Action onEnterRoomFailed;
        public event Action onEnteredRoom;
        public event Action<int> onOtherEnteredRoom;

        public event Action onMasterClientUpdated;
        public event Action<GameEventCode, object[]> onGameEventReceived;
        public event Action<ushort> onSyncMsgReceived;

        public Dictionary<int, GameEntity> networkObjectMap = new Dictionary<int, GameEntity>();
        public Dictionary<string, IRTSyncMsgReceiver> syncMsgReceivers = new Dictionary<string, IRTSyncMsgReceiver>();
        public ushort MIN = 2;
        public ushort MAX = 2;

        public event Action<GameEntity> onNetworkObjectSpawned;

        Dictionary<ushort, Queue<PropSyncField>> syncPropMsgQueue = new Dictionary<ushort, Queue<PropSyncField>>();

        public void Init(IRTNetworkClient client)
        {
            this.client = client;
            client.onEnteredRoom += OnEnteredRoom;
            client.onOtherEnteredRoom += OnOtherEnteredRoom;
            client.onCreateRoomFailed += OnCreateRoomFailed;
            client.onEnterRoomFailed += OnEnterRoomFailed;

            client.onGameEventReceived += ReceiveGameEvent;
        }

        public override void Dispose()
        {
            client.onEnteredRoom -= OnEnteredRoom;
            client.onOtherEnteredRoom -= OnOtherEnteredRoom;
            client.onCreateRoomFailed -= OnCreateRoomFailed;
            client.onEnterRoomFailed -= OnEnterRoomFailed;

            client.onGameEventReceived -= ReceiveGameEvent;
        }

        public void Login()
        {
            client.Login(region, appId);
            increment = 0;
        }

        public void Logout()
        {
            increment = 0;
            foreach (GameEntity entity in networkObjectMap.Values)
            {
                if (!entity.IsDisposed)
                    GameEntityDisposer.Dispose(entity);
            }
            syncMsgReceivers.Clear();
            networkObjectMap.Clear();
            syncPropMsgQueue.Clear();
            syncMsgBaseHandlers.Clear();
            client.Logout();
        }

        public void LeaveRoom()
        {
            client.LeaveRoom();
        }

        public void AutoJoin()
        {
            client.AutoJoin(MIN, MAX);
        }

        public List<ushort> GetOtherPlayerSessionIdList() => client.GetOtherPlayerSessionIdList();

        public ushort GetMySessionId() => client.GetMySessionId();

        public ushort GetRoomOwnerSessionId() => client.GetRoomOwnerSessionId();

        public bool IsRoomOwner() => client.IsRoomOwner();

        public bool IsRoomAtMax() => client.IsRoomAtMax();

        public int GetPlayerCountInRoom() => client.GetPlayerCountInRoom();

        public int GetIndexInRoom() => client.GetIndexInRoom();

        public ushort GetRandomPlayerSessionIdInRoom() => client.GetRandomPlayerSessionIdInRoom();

        public ushort GetYoungestPlayerSessionIdInRoom() => client.GetYoungestPlayerSessionIdInRoom();

        public bool IsYoungestPlayerSessionIdInRoom => client.IsYoungestPlayerSessionIdInRoom;

        public float NetworkFrameRate => client.NetworkFrameRate;

        public ushort GetInstancingId()
        {
            ushort id = (ushort)(GetMySessionId() * 100 + increment);
            increment++;
            return id;
        }

        public GameEntity FindNetworkObject(int networkObjectId) => networkObjectMap.ContainsKey(networkObjectId) ? networkObjectMap[networkObjectId] : null;

        void OnEnteredRoom() { if (onEnteredRoom != null) onEnteredRoom(); }
        void OnOtherEnteredRoom(int id) { if (onOtherEnteredRoom != null) onOtherEnteredRoom(id); }
        void OnCreateRoomFailed() { if (onCreateRoomFailed != null) onCreateRoomFailed(); }
        void OnEnterRoomFailed() { if (onEnterRoomFailed != null) onEnterRoomFailed(); }

        #region Sender

        Dictionary<GameEventCode, List<MsgHandler>> syncMsgBaseHandlers = new Dictionary<GameEventCode, List<MsgHandler>>();

        public MsgHandler SendGameEvent(GameEventCode gameEventCode, params object[] parameters)
        {
            LogSendGameEvent(gameEventCode, parameters);

            MsgHandler handler = new MsgHandler();

            if (!syncMsgBaseHandlers.ContainsKey(gameEventCode))
                syncMsgBaseHandlers.Add(gameEventCode, new List<MsgHandler>() { handler });

            syncMsgBaseHandlers[gameEventCode].Add(handler);

            if (ClientMode == ERTNetworkClientMode.ONLINE)
                client.SendGameEvent((ushort)gameEventCode, parameters);
            else if (ClientMode == ERTNetworkClientMode.OFFLINE)
                StartCoroutine(SendGameEventAtEndOfFrame(gameEventCode, parameters));

            return handler;
        }


        public async UniTask<GameEntity> SendSpawnNetworkObjectAsync(string assetPath, Vector3 position = default(Vector3))
        {
            ushort networkInstanceId = GetInstancingId();
            await SendGameEvent(GameEventCode.GAMEOBJECT_INSTANCING, assetPath, GetMySessionId(), networkInstanceId, position).SetCompleteCondition(() => networkObjectMap.ContainsKey(networkInstanceId));
            return networkObjectMap[networkInstanceId];
        }

        public void SendSpawnNetworkObject(string assetPath, Vector3 position = default(Vector3))
        {
            ushort networkInstanceId = GetInstancingId();
            SendGameEvent(GameEventCode.GAMEOBJECT_INSTANCING, assetPath, GetMySessionId(), networkInstanceId, position).SetCompleteCondition(() => networkObjectMap.ContainsKey(networkInstanceId));
        }


        /// <summary>
        /// 完了条件が自動的に決定される。完了条件は（カウンター数＝＝部屋のプレイヤー人数）
        /// </summary>
        public MsgHandler SendSyncMsgAsync(ushort code)
        {
            return SendGameEvent(GameEventCode.SYNC_MSG, code);
        }

        public void SendSyncMsg(ushort code)
        {
            SendGameEvent(GameEventCode.SYNC_MSG, code);
        }

        #endregion

        #region Reciever
        void ReceiveGameEvent(ushort gameEventCode, object[] p) => ReceiveGameEventAsync(gameEventCode, p);
        async void ReceiveGameEventAsync(ushort gameEventCode, object[] p)
        {
            //SYSTEM EVENTS
            GameEventCode code = (GameEventCode)gameEventCode;
            switch (code)
            {
                case GameEventCode.MASTER_CLIENT:
                    ReceiveUpdateMasterClient((ushort)p[0]);
                    break;
                case GameEventCode.TRANSFORM_POSITION_SYNC:
                    ReceiveSetTransform((ushort)p[0], (Vector2)p[1]);
                    break;
                case GameEventCode.TRANSFORM_ROTATION_SYNC:
                    ReceiveSetRotation((ushort)p[0], (Vector3)p[1]);
                    break;
                case GameEventCode.GAMEOBJECT_INSTANCING:
                    await ReceiveInstantiateNetworkObjectAsync((string)p[0], (ushort)p[1], (ushort)p[2], (Vector3)p[3]);
                    break;
                case GameEventCode.GAMEOBJECT_DESTROY:
                    ReceiveDestroyNetworkObject((ushort)p[0]);
                    break;
                case GameEventCode.RPC:
                    //ReceiveRPC(parameters);
                    break;
                case GameEventCode.PROP_SYNC:
                    ReceivePropertySync((ushort)p[0], (string)p[1], p[2]);
                    break;
                case GameEventCode.SYNC_MSG:
                    ReceiveSyncMsg((ushort)p[0]);
                    break;
            }

            if (syncMsgBaseHandlers.ContainsKey(code))
            {
                syncMsgBaseHandlers[code].ForEach(e => e.Handle());
                syncMsgBaseHandlers[code].RemoveAll(e => e.isComplete);
            }

            //GAME UNIQUE EVENTS
            if (onGameEventReceived != null)
                onGameEventReceived(code, p);
        }

        void ReceiveUpdateMasterClient(ushort sessionId)
        {
            Debug.Log($"<color=yellow>EVENT RECEIVED [Update master client]  sessionId = {sessionId}</color>");
            _isMasterClient = GetMySessionId() == sessionId;
            if (onMasterClientUpdated != null)
                onMasterClientUpdated();
        }

        //void ReceiveInstantiateNetworkObject(string rcName, ushort ownerSessionId, ushort networkInstanceId) => ReceiveInstantiateNetworkObjectAsync(rcName, ownerSessionId, networkInstanceId);

        async UniTask ReceiveInstantiateNetworkObjectAsync(string rcName, ushort ownerSessionId, ushort networkInstanceId, Vector3 position)
        {
            Debug.Log($"<color=yellow>EVENT RECEIVED [Instanciate gameobject]  rcName = {rcName}, ownerSessionId = {ownerSessionId}, networkInstanceId = {networkInstanceId}, position = {position}</color>");

            GameEntity entity = await GameNetworkObjectSpawner.SpawnAsync(rcName, ownerSessionId, networkInstanceId, ownerSessionId == RTNetworkClient.Instance.GetMySessionId(), position);

            //正常
            if (!networkObjectMap.ContainsKey(entity.networkInstanceId))
                networkObjectMap.Add(entity.networkInstanceId, entity);

            //エラー：既に同一IDでインスタンスIDが存在する
            else
            {
                Debug.LogError($"Error : same network instance Id[{networkInstanceId}] already exist");
                return;
            }
            //Debug.LogError("networkObjectMap.ContainsKey : " + entity.networkInstanceId + " / " + networkObjectMap.ContainsKey(entity.networkInstanceId));
            //メッセージキューの処理
            if (syncPropMsgQueue.ContainsKey(networkInstanceId))
            {
                while (syncPropMsgQueue[networkInstanceId].Count > 0)
                {
                    PropSyncField pf = syncPropMsgQueue[networkInstanceId].Dequeue();
                    entity.BroadcastPropertySync(pf.propertyName, pf.value);
                }
                syncPropMsgQueue.Remove(networkInstanceId);
            }

            if (onNetworkObjectSpawned != null)
                onNetworkObjectSpawned(entity);
        }

        void ReceiveDestroyNetworkObject(ushort networkInstanceId)
        {
            Debug.Log($"<color=yellow>EVENT RECEIVED [destroy gameobject]  networkInstanceId = {networkInstanceId}</color>");
            if (networkObjectMap.ContainsKey(networkInstanceId))
            {
                GameEntityDisposer.Dispose(networkObjectMap[networkInstanceId]);
                networkObjectMap.Remove(networkInstanceId);
            }
            else
                Debug.LogError($"Destroy networkobject error : cannot find networkobject[id={networkInstanceId}] in dictionary.");
        }

        void ReceiveSetTransform(ushort networkInstanceId, UnityEngine.Vector2 position)
        {
            //Debug.Log($"pos = ({position.x}, {position.y}) / id = {networkInstanceId}");
            if (networkObjectMap.ContainsKey(networkInstanceId))
            {
                if (!networkObjectMap[networkInstanceId].isOwner)
                    networkObjectMap[networkInstanceId].SetTransform(position);
            }
        }
        void ReceiveSetRotation(ushort networkInstanceId, UnityEngine.Vector3 eulerAngles)
        {
            if (networkObjectMap.ContainsKey(networkInstanceId))
            {
                if (!networkObjectMap[networkInstanceId].isOwner)
                    networkObjectMap[networkInstanceId].SetRotation(eulerAngles);
            }
        }

        void ReceiveSyncMsg(ushort syncMsgId)
        {
            Debug.Log($"<color=yellow>EVENT RECEIVED [sync message]  syncMsgId = {syncMsgId}</color>");

            foreach (IRTSyncMsgReceiver v in syncMsgReceivers.Values)
                v.ReceiveSyncMsg(syncMsgId);

            if (onSyncMsgReceived != null)
                onSyncMsgReceived(syncMsgId);
        }

        //TODO: not ready yet
        //void ReceiveRPC(ushort networkInstanceId, string funcName, object[] parameters)
        //{

        //    //指定インスタンスを検索
        //    if (!networkObjectMap.ContainsKey(networkInstanceId))
        //        return;

        //    GameEntity e = networkObjectMap[networkInstanceId];
        //    e.BroadcastRPC(funcName, parameters);
        //}

        void ReceivePropertySync(ushort networkInstanceId, string propertyName, object value)
        {
            Debug.Log($"<color=yellow>EVENT RECEIVED [Property Sync] networkInstanceId = {networkInstanceId} propertyName = {propertyName} value = {value}</color>");

            //指定インスタンスを検索、なければメッセージキューに追加
            if (!networkObjectMap.ContainsKey(networkInstanceId))
            {
                QueueMessage(networkInstanceId, propertyName, value);
                return;
            }

            GameEntity e = networkObjectMap[networkInstanceId];
            e.BroadcastPropertySync(propertyName, value);
        }



        void QueueMessage(ushort networkInstanceId, string propertyName, object value)
        {
            PropSyncField pf = new PropSyncField() { propertyName = propertyName, value = value, timeStamp = Time.time };

            if (!syncPropMsgQueue.ContainsKey(networkInstanceId))
                syncPropMsgQueue.Add(networkInstanceId, new Queue<PropSyncField>());

            syncPropMsgQueue[networkInstanceId].Enqueue(pf);
        }

        #endregion

        #region SyncMsgReceivers


        public void ResetSyncMsgReceivers()
        {
            foreach (IRTSyncMsgReceiver v in syncMsgReceivers.Values)
                v.Reset();
        }

        public void RegesterSyncMsgReceiver(string key, IRTSyncMsgReceiver syncMsgReceiver)
        {
            if (syncMsgReceivers.ContainsKey(key))
                return;

            syncMsgReceivers.Add(key, syncMsgReceiver);
        }

        public void RemoveSyncMsgReceiver(string key)
        {
            if (syncMsgReceivers.ContainsKey(key))
                syncMsgReceivers.Remove(key);
        }


        #endregion

        void Update()
        {
            client?.Update();
        }

        IEnumerator SendGameEventAtEndOfFrame(GameEventCode gameEventCode, params object[] parameters)
        {
            yield return new UnityEngine.WaitForEndOfFrame();
            client.SendGameEvent((ushort)gameEventCode, parameters);
        }


        void LogSendGameEvent(GameEventCode gameEventCode, params object[] parameters)
        {
            if (!(gameEventCode == GameEventCode.TRANSFORM_POSITION_SYNC || gameEventCode == GameEventCode.TRANSFORM_ROTATION_SYNC))
            {
                string log = "";
                for (int i = 0; i < parameters.Length; i++)
                    log += $"[{i}] = {parameters[i]}, ";

                Debug.Log($"<color=green>SEND EVENT [{gameEventCode}] : {log} / time : {Time.time}</color>");
            }
        }



    }
}