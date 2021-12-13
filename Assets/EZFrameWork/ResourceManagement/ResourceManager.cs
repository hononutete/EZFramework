
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EZFramework
{

    /// <summary>
    /// ロード完了ステータス
    /// </summary>
    public enum ELoadStatus
    {
        SUCCEEDED, FAILED
    }

    public enum EResourceConfig
    {

    }

    /// <summary>
    /// リソースの読み込みを行う。基本的にはアセットバンドルから読み込む。
    /// </summary>
    /// 
    public static class ResourceManager
    {
        static Dictionary<string, AsyncOperationHandle> handles = new Dictionary<string, AsyncOperationHandle>();

        //public static string imagesAssetBundleName;

#if UNITY_2019_3_OR_NEWER

        public static void GetDownloadSizeByLabelTag(string label, Action<long> onSuccess = null, Action onFailed = null)
        {
            //ダウンロード容量確認、引数はアドレスでもタグでもどっちでもいいみたいだ
            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(label);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    if (onSuccess != null)
                        onSuccess(handle.Result);
                }
                else
                {
                    Debug.LogError($"Failed to get download size of asset tagged as [{label}]");
                    Addressables.Release(handle);
                    if (onFailed != null)
                        onFailed();
                }
            };
        }

        public static async UniTask<long> GetDownloadSizeByLabelTagAsync(string label)
        {
            //ダウンロード容量確認、引数はアドレスでもタグでもどっちでもいいみたいだ
            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(label);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
                //if (onSuccess != null)
                //    onSuccess(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to get download size of asset tagged as [{label}]");
                Addressables.Release(handle);
                return 0;
                //if (onFailed != null)
                //    onFailed();
            }
        }

        /// <summary>
        /// タグによるリソースの事前ロード、例えば初回ダウンロードではこれを実行することで指定したタグのアセットをまとめてダウンロードなどができる。
        /// ダウンロードしたアセットはキャッシュに保存され、移行はキャッシュから展開される。TODO: 別スレッドでawaitするように読み込めるように
        /// </summary>
        public static void PreloadAssetByTag(string label, Action onSuccess = null, Action onFailed = null)
        {
            //ダウンロード容量確認、引数はアドレスでもタグでもどっちでもいいみたいだ
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(label);

            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    if (onSuccess != null)
                        onSuccess();
                }
                else
                {
                    Debug.LogError($"Failed to preload addressable asset by tag [{label}]");
                    Addressables.Release(handle);
                    if (onFailed != null)
                        onFailed();
                }
            };
        }

        /// <summary>
        /// Adressablesを利用したリソースのロード,　サーバーURLなどの設定は全てUnityのUIインターフェイスからできる。
        /// 注意！ロードできるオブジェクトはGameObjectに限られる。カスタムクラス（UIPanel)のタイプでロードしようとしたらできなかった
        /// </summary>
        public static void LoadAsync<T>(string address, Action<T> onSuccess, Action onFailed = null) where T : UnityEngine.Object
        {
            //ハンドルリストにキャッシュがあればそれを使う
            if (handles.ContainsKey(address))
            {
                //Debug.Log($"load addressable asset hit cache (lamda)[{address}]");
                if (onSuccess != null)
                    onSuccess(handles[address].Convert<T>().Result);
                return;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                //Debug.Log($"load addressable asset success (lamda)[{address}]");

                if (onSuccess != null)
                        onSuccess(op.Result);

                //ハンドルリストにキャッシュ
                if (!handles.ContainsKey(address))
                        handles.Add(address, handle);
                }
                else
                {
                //Debug.LogError($"Failed to load addressable asset (lamda)[{address}]");
                Addressables.Release(handle);
                    if (onFailed != null)
                        onFailed();
                }
            };
        }

        /// <summary>
        /// タスク版
        /// </summary>
        //public static async Task LoadAsyncTask<T>(string address, Action<T> onSuccess = null) where T : UnityEngine.Object
        //{
        //    //ハンドルリストにキャッシュがあればそれを使う
        //    if (handles.ContainsKey(address))
        //    {
        //        if (onSuccess != null)
        //            onSuccess(handles[address].Convert<T>().Result);
        //        return;
        //    }

        //    AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        //    await handle.Task;
        //    if (handle.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        Debug.Log($"load addressable asset success (threading)[{address}]");
        //        if (onSuccess != null)
        //            onSuccess(handle.Result);

        //        //ハンドルリストにキャッシュ
        //        if (!handles.ContainsKey(address))
        //            handles.Add(address, handle);
        //    }
        //    else
        //    {
        //        Debug.LogError($"Failed to load addressable asset (threading)[{address}]");
        //        Addressables.Release(handle);
        //    }
        //}

        public static T LoadSync<T>(string address) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T t = AssetDatabase.LoadAssetAtPath<T>(address);
            return t;
#else
        Debug.LogError("sync resource load is not allowed in this platform, use LoadAsync instead");
        return null;
#endif

        }

        public static async UniTask<T> LoadAsync<T>(string address) where T : UnityEngine.Object
        {
            //ハンドルリストにキャッシュがあればそれを使う
            if (handles.ContainsKey(address))
            {
                //Debug.Log($"load addressable asset hit cache (threading)[{address}]");
                return handles[address].Convert<T>().Result;
            }

            //TODO:キャッシュのコントロール
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //Debug.Log($"load addressable asset success (threading)[{address}]");

                //ハンドルリストにキャッシュ
                if (!handles.ContainsKey(address))
                    handles.Add(address, handle);

                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset (threading)[{address}]");
                Addressables.Release(handle);
                return null;
            }
        }

        /// <summary>
        /// Addressablesを利用したリソースのロードとインスタンス化を行う。
        /// </summary>
        public static void InstantiateAsync(string address, Transform parent, Action<GameObject> onSuccess, Action onFailed = null)
        {
            LoadAsync<GameObject>(address, (resRef) =>
            {
                //インスタンスを作成
                GameObject instance = UnityEngine.Object.Instantiate<GameObject>(resRef, parent);
                if (onSuccess != null)
                    onSuccess(instance);
            }, () =>
            {
                if (onFailed != null)
                    onFailed();
            });
        }

        /// <summary>
        /// タスク版
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string address, Transform parent = null)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
            GameObject objref = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject instance = UnityEngine.Object.Instantiate<GameObject>(objref, parent);
                return instance;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// タスク版
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string address, Transform parent, Vector3 localPos)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
            GameObject objref = await handle.Task;
            GameObject instance = UnityEngine.Object.Instantiate<GameObject>(objref, parent);
            instance.transform.localPosition = localPos;
            return instance;
        }

        public static async UniTask<GameObject> InstantiateAsync(string address, Transform parent, Vector3 localPos, Vector3 localEulerAngles)
        {
            GameObject instance = await InstantiateAsync(address, parent, localPos);
            instance.transform.localEulerAngles = localEulerAngles;
            return instance;
        }

#else

    /// <summary>
    /// 画像をロードする
    /// </summary>
    public static Sprite LoadSprite(string fileName)
    {
        //画像リソースはひとつの画像がひとつのアセットバンドルにパックされる
        return Load<Sprite>(fileName, fileName);
    }

    /// <summary>
    /// リソースのロード
    /// </summary>
    public static GameObject LoadPrefab(string prefabPath, string assetName, Transform parent = null)
    {
        GameObject objRef = Load<GameObject>(AssetBundleConfig.PREFAB_PATH + prefabPath, assetName);
        return UnityEngine.Object.Instantiate<GameObject>(objRef, parent);
    }

    public static T Load<T>(string assetBundlePath, string assetName) where T : UnityEngine.Object
    {
#if VIRTUAL_AB
        return LoadFromLocal<T>(assetBundlePath);
#else
        return LoadFromAssetBundle<T>(assetBundlePath, assetName);
#endif
    }

    /// <summary>
    /// ローカルに配置してある元リソースを読み込む
    /// </summary>
    static T LoadFromLocal<T>(string assetBundlePath) where T : UnityEngine.Object
    {
        return Resources.Load<T>(assetBundlePath);
    }

    /// <summary>
    /// アセットバンドルから読む込む。アセットバンドルのロード形式、圧縮方法などはここでは関与しない
    /// </summary>
    static T LoadFromAssetBundle<T>(string assetBundlePath, string assetName) where T : UnityEngine.Object
    {
        //アセットバンドルを読む込む
        AssetBundle assetBundle = AssetBundleManager.Instance.Load(assetBundlePath);

        if (assetBundle == null)
            return default(T);

        //読み込みが完了したアセットバンドルから目標のリソースを読み込む
        return assetBundle.LoadAsset<T>(assetName);
    }

    /// <summary>
    /// 同期でリソースを読み込む
    /// </summary>
    static T Load<T>(string assetName) where T : UnityEngine.Object
    {
        //仮想的にローカルに配置してある元リソースを読み込む

        //アセットバンドルから読む込む

        return default(T);
    }

        #region App indivisual loader

    /// <summary>
    /// battle prefabをロードする
    /// </summary>
    public static GameObject LoadBattlePrefab(string assetName, Transform parent = null)
    {
        //prefabリソースはひとつのprefabがひとつのアセットバンドルにパックされる
        GameObject objRef = Load<GameObject>(AssetBundleConfig.PREFAB_BATTLE_PATH + assetName, assetName);
        return UnityEngine.Object.Instantiate<GameObject>(objRef, parent);
    }

    /// <summary>
    /// uiprefabをロードする
    /// </summary>
    public static GameObject LoadUIPrefab(string assetName, Transform parent = null)
    {
        //prefabリソースはひとつのprefabがひとつのアセットバンドルにパックされる
        GameObject objRef = Load<GameObject>(AssetBundleConfig.PREFAB_UI_PATH + assetName, assetName);
        return UnityEngine.Object.Instantiate<GameObject>(objRef, parent);
    }

        #endregion
#endif
    }
}