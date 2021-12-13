using System;

namespace EZFramework
{
    public class MsgHandler
    {
        public bool isComplete = false;
        Action onComplete = null;

        //TODO: onCompleteとCombineするか検討
        Action onCompleteEvent = null;

        Func<bool> condition = null;

        public static MsgHandler Create()
        {
            return new MsgHandler();
        }

        public void Handle()
        {
            if (condition != null && !condition())
                return;

            //これを見てハンドラーリストから削除する
            isComplete = true;

            //awaiterに完了を通知するためのコールバック
            if (onComplete != null)
            {
                onComplete();
                onComplete = null;
            }

            //event用
            if (onCompleteEvent != null)
            {
                onCompleteEvent();
                onCompleteEvent = null;
            }

        }

        public bool IsComplete()
        {
            return false;
        }

        public void OnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
        }

        /// <summary>
        /// ハンドルすることで指定した条件が完了するまで待機する。指定しなければ待機しない。
        /// </summary>
        public MsgHandler SetCompleteCondition(Func<bool> condition)
        {
            this.condition = condition;
            return this;
        }

        public void Reset()
        {
            isComplete = false;
            onComplete = null;
            condition = null;
        }

        public void AddEvent(Action onCompleteEvent)
        {
            this.onCompleteEvent = onCompleteEvent;
        }


    }
}