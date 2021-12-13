using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

namespace EZFramework.API
{
    public class HttpClient : MonoBehaviour
    {
        public enum APIResponseState
        {
            SUCCESS, DATA_ERROR, SYS_ERROR
        }

        const long RESPONSE_CODE_SUCCESS = 200;
        const long RESPONSE_CODE_REDIRECTION = 300;
        const long RESPONSE_CODE_CLIENT_ERROR = 400;
        const long RESPONSE_CODE_SERVER_ERROR = 500;

        public Action<string> onSuccess;
        public Action<string> onSysError;
        public Action<string> onDataError;

        public string appServerURL = "";

        /// <summary>
        /// フォームを使ったPUT 
        /// </summary>
        public IEnumerator Put(APIRequest aPIRequest)
        {
            //フォーム作成
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
                form.AddField(pair.Key, pair.Value);

            UnityWebRequest request = UnityWebRequest.Post(appServerURL + aPIRequest.Url, form);
            request.method = UnityWebRequest.kHttpVerbPUT;
            request.downloadHandler = new DownloadHandlerBuffer();

            //ヘッダーセット
            foreach (KeyValuePair<string, string> pair in aPIRequest.HeaderData)
                request.SetRequestHeader(pair.Key, pair.Value);

            //フォームデータをURLの最後に追加
            string f = "?";
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
            {
                f += $"&{pair.Key}={pair.Value}";
            }
            request.url += f;

            request.ToString();
            Debug.Log("[API info] url : " + request.url);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("API Error :" + request.error);
                if (onSysError != null)
                    onSysError(request.error);
                yield break;
            }
            else
            {
                //TODO:まとめる
                if (request.responseCode >= RESPONSE_CODE_SERVER_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_CLIENT_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_REDIRECTION)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_SUCCESS)
                {
                    Debug.Log($"API Success status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onSuccess != null)
                        onSuccess(request.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// フォームを使ったポスト
        /// </summary>
        public IEnumerator Get(APIRequest aPIRequest)
        {
            //フォーム作成
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
                form.AddField(pair.Key, pair.Value);


            UnityWebRequest request = UnityWebRequest.Post(appServerURL + aPIRequest.Url, "");
            request.method = UnityWebRequest.kHttpVerbGET;
            request.downloadHandler = new DownloadHandlerBuffer();

            //ヘッダーセット
            foreach (KeyValuePair<string, string> pair in aPIRequest.HeaderData)
                request.SetRequestHeader(pair.Key, pair.Value);

            //フォームデータをURLの最後に追加
            string f = "?";
            //int i = 0;
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
            {
                f += $"&{pair.Key}={pair.Value}";
            }
            request.url += f;

            request.ToString();
            Debug.Log("[API info] url : " + request.url);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("API Error :" + request.error);
                if (onSysError != null)
                    onSysError(request.error);
                yield break;
            }
            else
            {
                //TODO:まとめる
                if (request.responseCode >= RESPONSE_CODE_SERVER_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_CLIENT_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_REDIRECTION)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_SUCCESS)
                {
                    Debug.Log($"API Success status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onSuccess != null)
                        onSuccess(request.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// フォームを使ったポスト
        /// </summary>
        public IEnumerator Post(APIRequest aPIRequest)
        {
            //フォーム作成
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
                form.AddField(pair.Key, pair.Value);

            UnityWebRequest request = UnityWebRequest.Post(appServerURL + aPIRequest.Url, form);

            request.downloadHandler = new DownloadHandlerBuffer();

            //ヘッダーセット
            foreach (KeyValuePair<string, string> pair in aPIRequest.HeaderData)
                request.SetRequestHeader(pair.Key, pair.Value);

            request.ToString();
            Debug.Log("[API info] url : " + request.url);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("API Error :" + request.error);
                if (onSysError != null)
                    onSysError(request.error);
                yield break;
            }
            else
            {
                //TODO:まとめる
                if (request.responseCode >= RESPONSE_CODE_SERVER_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_CLIENT_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_REDIRECTION)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_SUCCESS)
                {
                    Debug.Log($"API Success status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onSuccess != null)
                        onSuccess(request.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// delete
        /// </summary>
        public IEnumerator Delete(APIRequest aPIRequest)
        {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> pair in aPIRequest.FormData)
                form.AddField(pair.Key, pair.Value);

            UnityWebRequest request = UnityWebRequest.Post(appServerURL + aPIRequest.Url, form);
            request.method = UnityWebRequest.kHttpVerbDELETE;
            request.downloadHandler = new DownloadHandlerBuffer();

            //ヘッダーセット
            foreach (KeyValuePair<string, string> pair in aPIRequest.HeaderData)
                request.SetRequestHeader(pair.Key, pair.Value);

            request.ToString();
            Debug.Log("[API info] url : " + request.url);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("API Error :" + request.error);
                if (onSysError != null)
                    onSysError(request.error);
                yield break;
            }
            else
            {
                if (request.responseCode >= RESPONSE_CODE_SERVER_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_CLIENT_ERROR)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_REDIRECTION)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
                else if (request.responseCode >= RESPONSE_CODE_SUCCESS)
                {
                    Debug.Log($"API Error status code = {request.responseCode} url = {aPIRequest.Url} = {request.downloadHandler.text}");
                    if (onSuccess != null)
                        onSuccess(request.downloadHandler.text);
                }
            }
        }

        public IEnumerator DownloadFile(string url, string savePath)
        {
            UnityWebRequest request = new UnityWebRequest(url);
            request.downloadHandler = new DownloadHandlerFile(savePath);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("API Error :" + request.error);
                if (onSysError != null)
                    onSysError(request.error);
                yield break;
            }
            else
            {
                if (request.responseCode == RESPONSE_CODE_SUCCESS)
                {
                    Debug.Log("API success :" + url + " = " + request.downloadHandler.text);
                    if (onSuccess != null)
                        onSuccess(request.downloadHandler.text);
                }
                else
                {
                    Debug.Log("API Error :" + url + " = " + request.downloadHandler.text);
                    if (onDataError != null)
                        onDataError(request.downloadHandler.text);
                }
            }
        }



    }
}
