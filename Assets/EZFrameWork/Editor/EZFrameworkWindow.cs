using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EZFramework.Editor
{
    public class EZFrameWorkWindow : EditorWindow
    {
        string masterdataPath;

        [MenuItem("Window/EZFramework")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            EZFrameWorkWindow window = (EZFrameWorkWindow)EditorWindow.GetWindow(typeof(EZFrameWorkWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            masterdataPath = EditorGUILayout.TextField("MasterData Path", masterdataPath);

        }
    }
}
