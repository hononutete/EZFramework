using UnityEditor;
using System.IO;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

namespace EZFramework.Editor
{
    public class CreateMasterDataClass
    {
        static bool waitingForReload = false;

        /// <summary>
        /// マスターモデルクラスを元にローダークラスを自動生成する
        /// </summary>
        [MenuItem("Tools/Create Master Class")]
        public static void CreateMasterClass()
        {
            //セッティング情報読み込み
            EZFrameWorkSettings ezSettings = AssetDatabase.LoadAssetAtPath<EZFrameWorkSettings>("Assets/EZFrameWork/EZFrameWorkSettings.asset");

            //MasterModelAttributeを持ったクラス一覧を取得
            List<Type> masterModelTypes = GetAllClassesOfAttribute<MasterModelAttribute>();

            //マスターデータクラスファイルの作成
            string filePath = Application.dataPath + "/" + ezSettings.MasterClassOutputPath + "MasterData.cs";
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            using (writer)
            {
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using UnityEngine;\n");

                writer.WriteLine("public partial class MasterData : ScriptableObject");
                writer.WriteLine("{");

                //staticなインスタンス変数を定義
                //writer.WriteLine($"\tpublic static MasterData Instance;");

                //テーブルリストを定義
                foreach (Type type in masterModelTypes)
                {
                    writer.WriteLine($"\tpublic List<{type.Name}> {type.Name}s;");
                    FieldInfo fi = type.GetField("ID");
                    string idType = fi.FieldType.ToString();
                    writer.WriteLine($"\tpublic Dictionary<{idType}, {type.Name}> {type.Name}Dic;\n");
                }

                //実行時にListをDictionaryに変換する関数
                writer.WriteLine("\tpublic void ConvertToDictionary(){");
                foreach (Type type in masterModelTypes)
                {
                    writer.WriteLine($"\t\t{type.Name}Dic = {type.Name}s.ToDictionary(e => e.ID, e => e);");
                }
                writer.WriteLine("\t}\n");


                //テーブルにデータをセットする関数
                writer.WriteLine("\tpublic void LoadCsv(string basePath){");
                foreach (Type type in masterModelTypes)
                {
                    string csvFileName = type.Name + ".csv";
                    string csvFilePath = "/" + ezSettings.MasterCsvInputPath + csvFileName;
                    writer.WriteLine($"\t\t{type.Name}s = CSVParser.Parse<{type.Name}>(basePath + \"{csvFilePath}\");");

                }
                writer.WriteLine("\t}");

                writer.WriteLine("} ");

            }

            waitingForReload = true;
            AssetDatabase.Refresh();
        }


        [MenuItem("Tools/Create Master Asset")]
        public static void CreateMasterAsset()
        {
            //セッティング情報読み込み
            EZFrameWorkSettings ezSettings = AssetDatabase.LoadAssetAtPath<EZFrameWorkSettings>("Assets/EZFrameWork/EZFrameWorkSettings.asset");

            //マスターデータクラスのインスタンス（ScriptableObject）を作成し、リフレクションを使ってデータをセットする。
            MasterData master = ScriptableObject.CreateInstance<MasterData>();
            master.LoadCSV(Application.dataPath);

            AssetDatabase.CreateAsset(master, "Assets/AddressableAssets/" + ezSettings.AddressableMasterAssetOutputPath + "masterdata.asset");

            //アドレッサブルに登録
            AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.Settings;

            //グループ取得（TODO:なければ作成）
            AddressableAssetGroup group = aaSettings.FindGroup("common Assets");

            //マスターアセットのGUIDを取得
            string guid = AssetDatabase.AssetPathToGUID("Assets/AddressableAssets/" + ezSettings.AddressableMasterAssetOutputPath + "masterdata.asset");

            //登録
            AddressableAssetEntry entry = aaSettings.CreateOrMoveEntry(guid, group);

            aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);

        }


        static List<Type> GetAllClassesOfAttribute<T>() where T : Attribute
        {

            Assembly asm = Assembly.Load("Assembly-CSharp");

            List<Type> typeBuffer = new List<Type>();

            Type[] types = asm.GetTypes();
            foreach (Type type in types)
            {

                Attribute attribu = type.GetCustomAttribute<T>();
                if (attribu != null)
                {
                    typeBuffer.Add(type);
                }
            }
            return typeBuffer;
        }

        static void Csv2MasterAsset()
        {

        }

    }
}
