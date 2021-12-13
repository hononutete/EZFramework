using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace EZFramework.Game
{

    public static class SceneManager
    {
        public static async UniTask LoadSceneAsync(string sceneName)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            SceneInstance sceneInstance = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("load scene succeeded");

            }
            else
            {
                Debug.Log("load scene failed");
            }
        }

    }
}
