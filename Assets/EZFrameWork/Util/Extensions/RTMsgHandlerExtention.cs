namespace EZFramework
{
    public static class SyncMessageHandlerExtention
    {
        // TweenのAwaiter
        public struct MessageHandlerAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion
        {
            MsgHandler handler;

            public MessageHandlerAwaiter(MsgHandler handler) => this.handler = handler;

            // 最初にすでに終わってるのか終わってないのかの判定のために呼び出される
            public bool IsCompleted => handler.IsComplete();

            // Tweenは値を返さない
            public void GetResult() { }

            // このAwaiterの処理が終わったらcontinuationを呼び出すメソッド
            public void OnCompleted(System.Action continuation) => handler.OnComplete(() => continuation());

            // OnCompleted
            public void UnsafeOnCompleted(System.Action continuation) => handler.OnComplete(() => continuation());
        }

        // Tweenに対する拡張メソッド
        public static MessageHandlerAwaiter GetAwaiter(this MsgHandler self)
        {
            return new MessageHandlerAwaiter(self);
        }
    }
}
