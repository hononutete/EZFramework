using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EZFramework.Editor
{

    /// <summary>
    /// UnityEditor上でアセットを読み込む
    /// </summary>
    public static class EditorAssetLoader
    {
        /// <summary>
        /// ローカルに配置してある元リソースを読み込む
        /// </summary>
        public static T LoadFromLocal<T>(string path, string fileName) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath("Assets/" + path + fileName, typeof(T)) as T;
        }
    }
}
