using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Realtime
{

    public interface IRTNetworkClient
    {
        event Action onCreateRoomFailed;
        event Action onEnterRoomFailed;
        event Action onEnteredRoom;
        event Action<int> onOtherEnteredRoom;
        event Action<ushort, object[]> onGameEventReceived;


        void Init();

        void Login(ERegion region, string appId);

        void Logout();

        void AutoJoin(ushort min, ushort max);

        void LeaveRoom();

        ushort GetMySessionId();

        List<ushort> GetOtherPlayerSessionIdList();

        ushort GetRoomOwnerSessionId();

        bool IsRoomOwner();

        //Room GetCurrentRoom();

        bool IsRoomAtMax();

        int GetPlayerCountInRoom();

        int GetIndexInRoom();

        ushort GetRandomPlayerSessionIdInRoom();

        ushort GetYoungestPlayerSessionIdInRoom();

        bool IsYoungestPlayerSessionIdInRoom { get; }

        float NetworkFrameRate { get; }

        void SendGameEvent(ushort gameEventCode, object[] parameters);

        void Update();

    }
}
