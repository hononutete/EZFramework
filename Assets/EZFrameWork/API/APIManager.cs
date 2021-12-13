using System;
using EZFramework.Util;

namespace EZFramework.API
{
    public class APIManager : SingletonMonobehaviour<APIManager>
    {

        HttpClient httpClient;

        public void Init()
        {
            httpClient = gameObject.AddComponent<HttpClient>();
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// formを使ったpost
        /// </summary>
        public void Get(APIDesc aPIDesc, Action onSuccess = null, Action<HttpClient.APIResponseState> onFailed = null)
        {
            httpClient.onSuccess = (text) =>
            {
            //更新のあるプレイヤーデータを自動更新
            //aPIRequest.OnSuccess<V>(text);

            //ハンドラーに結果を処理してもらう
            if (aPIDesc.responseHandler != null)
                    aPIDesc.responseHandler.OnSuccess(text);

            //成功を利用者に通知、TODO:内容に応じて処理が変わる可能性があるからレスポンスも一緒に一応返す
            if (onSuccess != null) onSuccess();

            };

            httpClient.onDataError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.DATA_ERROR);
            };
            httpClient.onSysError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.SYS_ERROR);
            };

            StartCoroutine(httpClient.Get(aPIDesc.request));

        }

        /// <summary>
        /// formを使ったpost
        /// </summary>
        public void Put(APIDesc aPIDesc, Action onSuccess = null, Action<HttpClient.APIResponseState> onFailed = null)
        {
            httpClient.onSuccess = (text) =>
            {
            //更新のあるプレイヤーデータを自動更新
            //aPIRequest.OnSuccess<V>(text);

            //ハンドラーに結果を処理してもらう
            if (aPIDesc.responseHandler != null)
                    aPIDesc.responseHandler.OnSuccess(text);

            //成功を利用者に通知、TODO:内容に応じて処理が変わる可能性があるからレスポンスも一緒に一応返す
            if (onSuccess != null) onSuccess();

            };

            httpClient.onDataError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.DATA_ERROR);
            };
            httpClient.onSysError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.SYS_ERROR);
            };

            StartCoroutine(httpClient.Put(aPIDesc.request));

        }

        /// <summary>
        /// formを使ったpost
        /// </summary>
        public void Post(APIDesc aPIDesc, Action onSuccess = null, Action<HttpClient.APIResponseState> onFailed = null)
        {
            httpClient.onSuccess = (text) =>
            {
            //更新のあるプレイヤーデータを自動更新
            //aPIRequest.OnSuccess<V>(text);

            //ハンドラーに結果を処理してもらう
            if (aPIDesc.responseHandler != null)
                    aPIDesc.responseHandler.OnSuccess(text);

            //成功を利用者に通知、TODO:内容に応じて処理が変わる可能性があるからレスポンスも一緒に一応返す
            if (onSuccess != null) onSuccess();

            };

            httpClient.onDataError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.DATA_ERROR);
            };
            httpClient.onSysError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.SYS_ERROR);
            };

            StartCoroutine(httpClient.Post(aPIDesc.request));

        }

        /// <summary>
        /// formを使ったdelete
        /// </summary>
        public void Delete(APIDesc aPIDesc, Action onSuccess = null, Action<HttpClient.APIResponseState> onFailed = null)
        {
            httpClient.onSuccess = (text) =>
            {
            //ハンドラーに結果を処理してもらう
            if (aPIDesc.responseHandler != null)
                    aPIDesc.responseHandler.OnSuccess(text);

            //成功を利用者に通知、TODO:内容に応じて処理が変わる可能性があるからレスポンスも一緒に一応返す
            if (onSuccess != null) onSuccess();

            };

            httpClient.onDataError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.DATA_ERROR);
            };
            httpClient.onSysError = (text) =>
            {
                if (onFailed != null) onFailed(HttpClient.APIResponseState.SYS_ERROR);
            };

            StartCoroutine(httpClient.Delete(aPIDesc.request));

        }


        /// <summary>
        /// ファイルのダウンロードを行う
        /// </summary>
        public void DownloadFile(string downloadUrl, string savePath, Action<HttpClient.APIResponseState> onResponse = null)
        {
            HttpClient httpClientDownload = gameObject.AddComponent<HttpClient>();
            httpClientDownload.onSuccess = (text) =>
            {
                if (onResponse != null) onResponse(HttpClient.APIResponseState.SUCCESS);
            };
            httpClientDownload.onDataError = (text) =>
            {
                if (onResponse != null) onResponse(HttpClient.APIResponseState.DATA_ERROR);
            };
            httpClientDownload.onSysError = (text) =>
            {
                if (onResponse != null) onResponse(HttpClient.APIResponseState.SYS_ERROR);
            };

            StartCoroutine(httpClientDownload.DownloadFile(downloadUrl, savePath));
        }
    }
}