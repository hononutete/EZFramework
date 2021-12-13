using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using EZFramework.Util;

namespace EZFramework.UI
{

    /// <summary>
    /// UI全般を管理するクラス
    /// </summary>
    public class UIManager : SingletonMonobehaviour<UIManager>
    {
        public Canvas canvas { get; private set; }

        public static string baseAssetPath;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SetUpDefualt()
        {
            if (GameObject.Find("Canvas") == null)
            {
                //リソースからCanvasを読み込む。同期読み込みをする必要があるし、更新の必要がないからリソースを利用。
                GameObject objRef = Resources.Load<GameObject>("prefabs/UI/Canvas");
                GameObject instance = Instantiate<GameObject>(objRef);
                GameObject.DontDestroyOnLoad(Instance);
                Instance.Init();
            }
            else
            {
                GameObject.DontDestroyOnLoad(GameObject.Find("Canvas"));
            }

        }


        public Camera GetTargetCamera() => GetComponent<Canvas>().worldCamera;

        public void SetTargetCamera(Camera cam)
        {
            GetComponent<Canvas>().worldCamera = cam;
            GetComponent<Canvas>().planeDistance = 1.0f;
        }

        public void SetPlaneDistance(float distance)
        {
            GetComponent<Canvas>().planeDistance = distance;
        }

        public RenderMode CanvasRenderMode => GetComponent<Canvas>().renderMode;


        /// <summary>
        /// pushとpopをコントロールするためのstack
        /// </summary>
        public Stack<UIPanel> stack = new Stack<UIPanel>();

        public UIPanel CurrentActivePanel { get; private set; }
        //Transform panelRoot;
        public Transform panelRoot;

        public void Init()
        {
            if (panelRoot == null)
                panelRoot = transform.Find("Panel");

            canvas = GetComponent<Canvas>();

        }

        public T GetActive<T>() where T : UIPanel
        {
            return CurrentActivePanel as T;
        }

        /// <summary>
        /// パネルをロードして、パネルルート下に配置する。
        /// </summary>
        public void LoadPanelAsync<T>(Action<T> onSuccess = null) where T : UIPanel
        {
            ResourceManager.InstantiateAsync(baseAssetPath + typeof(T).ToString() + ".prefab", panelRoot, instance =>
            {
                instance.transform.localScale = Vector3.one;
                if (onSuccess != null)
                {
                    T panel = instance.GetComponent<T>();

                    Debug.Assert(panel != null, $"Panel component [{typeof(T).ToString()}] not found in addresable gameobject prefab");

                    instance.transform.localScale = Vector3.one;
                    instance.SetActive(false);

                    //動的にロードするパネルは、ロード済みパネルよりも必ず上になる。
                    instance.transform.SetAsFirstSibling();

                    panel.OnLoaded();

                    //Canvas.ForceUpdateCanvases();

                    if (panel != null && onSuccess != null)
                        onSuccess(panel);
                }
            });
        }

        public async UniTask<T> LoadPanelAsync<T>() where T : UIPanel
        {
            GameObject instance = await ResourceManager.InstantiateAsync(baseAssetPath + typeof(T).ToString() + ".prefab", panelRoot);
            T panel = instance.GetComponent<T>();

            Debug.Assert(panel != null, $"Panel component [{typeof(T).ToString()}] not found in addresable gameobject prefab");

            instance.transform.localScale = Vector3.one;
            instance.SetActive(false);

            panel.OnLoaded();

            //動的にロードするパネルは、ロード済みパネルよりも必ず上になる。
            instance.transform.SetAsFirstSibling();

            return panel;
        }


        public void HideAll()
        {
            foreach (Transform trans in transform)
            {
                UIPanel panel = trans.gameObject.GetComponent<UIPanel>();
                if (panel == null) continue;
                trans.gameObject.SetActive(false);
                trans.gameObject.GetComponent<UIPanel>().OnDeactivated();
            }
            CurrentActivePanel = null;
        }

        public void DestroyPanel<T>() where T : UIPanel
        {
            T panel = Get<T>();
            if (panel == null) return;

            panel.OnDispose();

            Destroy(panel.gameObject);
        }

        /// <summary>
        /// パネルを表示する
        /// </summary>
        public T Show<T>() where T : UIPanel
        {
            //前のパネルを非表示
            if (CurrentActivePanel != null)
            {
                DeactivatePanel(CurrentActivePanel);
            }

            //既にロード済みかどうか
            CurrentActivePanel = panelRoot.GetComponentInChildren<T>(true);
            if (CurrentActivePanel == null)
            {
                //TODO: 同期ロード関数を用意してここでロード
                Debug.Assert(CurrentActivePanel == null, $"UIPanel of Type [{typeof(T).ToString()}] is not Loaded.");
                return null;
            }

            //次のパネルを表示
            ActivatePanel(CurrentActivePanel);

            return CurrentActivePanel as T;
        }

        /// <summary>
        /// Panel取得。非アクティブのパネルも取得可能
        /// </summary>
        public T Get<T>() where T : UIPanel
        {
            //既にロード済みかどうか
            T panel = panelRoot.GetComponentInChildren<T>(true);
            if (panel == null)
            {
                //TODO: 同期ロード関数を用意してここでロード
                //Debug.Assert(CurrentActivePanel == null, $"UIPanel of Type [{typeof(T).ToString()}] is not Loaded.");
                return null;
            }
            return panel;
        }

        /// <summary>
        /// ActivePanelに関連しないコントロール。そのためパネルの状態は自分で管理する必要あり。
        /// </summary>
        public void ManualActivate<T>(bool isActive) where T : UIPanel
        {
            T panel = Get<T>();
            if (panel == null) return;

            //panel.gameObject.SetActive(isActive);
            if (isActive)
                ActivatePanel(panel);
            else
                DeactivatePanel(panel);
        }


        /// <summary>
        /// パネルをpushする。未実装
        /// </summary>
        public void Push<T>() where T : UIPanel
        {
            stack.Push(CurrentActivePanel);

            //前のパネル処理
            CurrentActivePanel.OnPushedOut();
            if (CurrentActivePanel.deactivateOnPush)
                DeactivatePanel(CurrentActivePanel);

            //既にロード済みかどうか
            CurrentActivePanel = panelRoot.GetComponentInChildren<T>(true);
            if (CurrentActivePanel == null)
            {
                //TODO: 同期ロード関数を用意してここでロード
                Debug.Assert(CurrentActivePanel == null, $"UIPanel of Type [{typeof(T).ToString()}] is not Loaded.");
                return;
            }

            ActivatePanel(CurrentActivePanel);
            CurrentActivePanel.OnPushed();
        }

        /// <summary>
        /// パネルをpopする。未実装
        /// </summary>
        public void Pop()
        {
            Debug.Assert(stack.Count > 0, $"Panel stack count must be larger than 0 to pop");

            CurrentActivePanel.OnPopped();
            DeactivatePanel(CurrentActivePanel);

            CurrentActivePanel = stack.Pop();
            CurrentActivePanel.OnPoppedIn();
            if (CurrentActivePanel.deactivateOnPush)
                ActivatePanel(CurrentActivePanel);

        }

        void ActivatePanel(UIPanel panel)
        {
            panel.gameObject.SetActive(true);
            if (!panel.hasActivatedOnce)
            {
                panel.OnActivatedFirstTime();
                panel.hasActivatedOnce = true;
            }
            panel.OnActivated();
        }

        void DeactivatePanel(UIPanel panel)
        {
            panel.OnDeactivated();
            panel.gameObject.SetActive(false);
        }

        public void ClearStack()
        {
            //TODO:パネルの破棄をするかどうか
            stack.Clear();
        }
    }
}
