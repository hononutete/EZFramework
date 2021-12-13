using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace EZFramework
{

    [CreateAssetMenu(fileName = "masterdata", menuName = "ScriptableObjects/MasterData", order = 1)]
    public partial class MasterData : ScriptableObject
    {
        public static MasterData Instance;


        public static async UniTask LoadAsync(string assetPath)
        {
            if (Instance == null)
            {
                Instance = await ResourceManager.LoadAsync<MasterData>(assetPath);
                Instance.ConvertToDictionary();
                Instance.FormatForQuery();
            }
        }

        public static void Clear()
        {
            Instance = null;
            //TODO: アドレッサブルの解放処理
        }

        public void LoadCSV(string path)
        {
            LoadCsv(path);
        }

        partial void LoadCsv(string path);

        partial void FormatForQuery();

        partial void ConvertToDictionary();
    }
}
