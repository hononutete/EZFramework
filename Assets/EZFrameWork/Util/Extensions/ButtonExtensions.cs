using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace EZFramework
{
    public static class ButtonExtensions
    {
        public static async UniTask WaitForClick(this Button button)
        {
            button.gameObject.SetActive(true);
            MsgHandler msgHandler = new MsgHandler();
            void OnClicked() => msgHandler.Handle();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);

            await msgHandler;

            button.gameObject.SetActive(false);
        }
    }
}
