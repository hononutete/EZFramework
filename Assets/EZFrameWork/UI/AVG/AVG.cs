using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EZFramework.UI;

namespace EZFramework.AVG
{
    public static class AVG
    {
        public static AVGScenario currentScenario { get; private set; }
        public static AVGScenarioView view;

        public static string characterImagePath;
        public static string frameImagePath;

        static MsgHandler onScenarioFinished;

        public static IAVGImageLoader imageLoader;
        static AVGScenarioLoader scenarioLoader;

        static bool isInitialized = false;

        public static void Init(AVGScenarioLoader sceloader, IAVGImageLoader imgLoader)
        {
            scenarioLoader = sceloader;
            imageLoader = imgLoader;
            isInitialized = true;
        }

        public static async UniTask Play(int scenarioId)
        {
            if (!isInitialized) return;
            await LoadScenario();

            Next();

            await onScenarioFinished;
        }

        static async UniTask<AVGScenario> LoadScenario()
        {
            if (!isInitialized) return null;

            //ビューのロード
            view = UIManager.Instance.Get<AVGScenarioView>();
            if (view == null)
                view = await UIManager.Instance.LoadPanelAsync<AVGScenarioView>();

            UIManager.Instance.ManualActivate<AVGScenarioView>(true);

            //データ取得
            List<MAvgScenarioCut> cutDatas = scenarioLoader.LoadScenarioCut();

            AVGScenario scenario = new AVGScenario(cutDatas);
            await scenario.Load();
            currentScenario = scenario;

            //ハンドラー
            onScenarioFinished = MsgHandler.Create();

            return scenario;
        }

        static void UnloadScenario()
        {
            onScenarioFinished = null;
            UIManager.Instance.ManualActivate<AVGScenarioView>(false);
            currentScenario.Dispose();
            currentScenario = null;
        }

        static void Next()
        {
            GoNext();

            if (currentScenario == null)
                return;

            //遷移条件
            switch (AVGScenario.EAIStateTransitionConditionType.TAP)
            {
                case AVGScenario.EAIStateTransitionConditionType.FORCE:
                    //三秒間で遷移
                    DOVirtual.DelayedCall(3.0f, () => Next());
                    break;
                case AVGScenario.EAIStateTransitionConditionType.TAP:
                    void OnTapped()
                    {
                        view.onTapped -= OnTapped;
                        Next();
                    }
                    view.onTapped += OnTapped;
                    break;

                default:
                    //三秒間で遷移
                    DOVirtual.DelayedCall(3.0f, () => Next());
                    break;
            }

        }

        static void GoNext()
        {
            //falseでシナリオ全部終了
            if (!currentScenario.Next())
            {
                onScenarioFinished?.Handle();

                UnloadScenario();
            }
        }
    }
}

