using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Realtime
{

    public class LocalRTNetworkClient : IRTNetworkClient
    {
        public event Action onCreateRoomFailed;
        public event Action onEnterRoomFailed;
        public event Action onEnteredRoom;
        public event Action<int> onOtherEnteredRoom;
        public event Action<ushort, object[]> onGameEventReceived;

        public void Init() { }

        public void Login(ERegion region, string appId) { }

        public void Logout() { }

        public void AutoJoin(ushort min, ushort max) { }

        public void LeaveRoom() { }

        public ushort GetMySessionId() => 1;

        public List<ushort> GetOtherPlayerSessionIdList() => new List<ushort>();

        public ushort GetRoomOwnerSessionId() => 1;

        public bool IsRoomOwner() => true;


        public bool IsRoomAtMax() => true;

        public int GetPlayerCountInRoom() => 1;

        public int GetIndexInRoom() => 0;

        public ushort GetRandomPlayerSessionIdInRoom() => 1;

        public ushort GetYoungestPlayerSessionIdInRoom() => 1;

        public bool IsYoungestPlayerSessionIdInRoom => true;

        public float NetworkFrameRate => 0;

        public void SendGameEvent(ushort gameEventCode, object[] parameters)
        {
            //１フレーム遅らせる
            if (onGameEventReceived != null)
                onGameEventReceived(gameEventCode, parameters);
        }

        public void Update() { }
    }
}
