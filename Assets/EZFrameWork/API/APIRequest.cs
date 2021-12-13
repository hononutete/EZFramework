using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.API
{
    /// <summary>
    /// APIの詳細を記述するクラス
    /// </summary>
    public abstract class APIRequest
    {
        public string Url { get; protected set; }

        public Dictionary<string, string> HeaderData { get; protected set; }

        public Dictionary<string, string> FormData { get; protected set; }

        protected APIRequest()
        {
            //Init();
        }

        public APIRequest Init()
        {
            FormData = new Dictionary<string, string>();
            HeaderData = new Dictionary<string, string>();
            SetHeaderData();
            SetFormData();
            SetURL();
            return this;
        }

        /// <summary>
        /// 共通ヘッダーデータをセット。変更が必要な場合はオーバーライド。
        /// </summary>
        protected abstract void SetHeaderData();

        /// <summary>
        /// 共通フォームデータをセット。変更が必要な場合はオーバーライド。
        /// </summary>
        protected abstract void SetFormData();

        protected abstract void SetURL();

        protected abstract void OnSuccess<T>(T result);
    }
}
