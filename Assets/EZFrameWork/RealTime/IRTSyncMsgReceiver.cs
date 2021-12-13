namespace EZFramework.Realtime
{
    public interface IRTSyncMsgReceiver
    {
        void ReceiveSyncMsg(ushort syncMsgId);
        void Reset();
    }
}
