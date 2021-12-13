using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EZFramework.Editor
{

    public static class AutoBuildSettings
    {

        private static readonly BuildTargetGroup[] targetGroup =
    {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android,
        BuildTargetGroup.iOS
    };

        static List<string> GetCurrentSymbols(BuildTargetGroup group)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(s => s.Trim()).ToList();
        }

        [MenuItem("Tools/BuildSettings/Set for iOS")]
        static void SetForMobile()
        {
            GetCurrentSymbols(BuildTargetGroup.iOS);
        }
    }
}