using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework
{
    public class BundleVersionInfo : ScriptableObject
    {
        /// <summary>
        /// 全てのバンドルのバージョン情報を保持
        /// </summary>
        public List<VersionInfo> versionInfos;
    }
}
