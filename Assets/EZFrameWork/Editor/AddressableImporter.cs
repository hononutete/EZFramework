using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

namespace EZFramework.Editor
{

    public class AddressableImporter : AssetPostprocessor
    {


        void OnPreprocessAsset()
        {
            if (!assetImporter.assetPath.Contains("."))
            {
                return;
            }

            if (assetImporter.assetPath.Contains("AddressableAssets/"))
            {
                string guid = AssetDatabase.AssetPathToGUID((assetImporter.assetPath));

                //アドレッサブルに登録
                AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.Settings;

                //ローカルグループはデフォルトのものを使用


                //タグ取得
                string labelpath = assetImporter.assetPath.Replace("Assets/AddressableAssets/", "");
                string label = labelpath.Split('/')[0];
                string groupName = $"{label} Assets";
                //リモートグループ取得
                AddressableAssetGroup group = aaSettings.FindGroup(groupName);
                if (group == null)
                {
                    //スキーマ生成
                    List<UnityEditor.AddressableAssets.Settings.AddressableAssetGroupSchema> schema = new List<UnityEditor.AddressableAssets.Settings.AddressableAssetGroupSchema>(){
                    new UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema(),
                   new UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema()
                };
                    group = aaSettings.CreateGroup(groupName, false, false, true, schema);
                }

                //同じパスでもguidが変わった場合複数登録される場合がある、元のエントリーを削除
                AddressableAssetEntry entry = aaSettings.FindAssetEntry(guid);

                //エントリー作成
                if (entry == null)
                {
                    Debug.Log($"{assetImporter.assetPath} was added to Addressable Group " + groupName);

                    entry = aaSettings.CreateOrMoveEntry(guid, group);

                    entry.SetAddress(labelpath);

                    //タグ生成
                    entry.SetLabel(label, true);
                    aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);

                }

            }
        }
    }
}