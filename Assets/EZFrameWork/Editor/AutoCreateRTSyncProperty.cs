using UnityEditor;
using System.IO;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using EZFramework.Realtime;

namespace EZFramework.Editor
{
    public class AutoCreateSyncProperty
    {
        /// <summary>
        /// マスターモデルクラスを元にローダークラスを自動生成する
        /// </summary>
        [MenuItem("Tools/Create Sync Extension Class")]
        public static void CreateMasterClass()
        {
            //MasterModelAttributeを持ったクラス一覧を取得
            Dictionary<Type, List<FieldInfo>> dic = GetAllClassesOfAttribute<RTSyncPropAttribute>();


            //テーブルリストを定義
            foreach (Type type in dic.Keys)
            {
                CreatePartialClass<RTSyncPropAttribute>(type);
            }

            AssetDatabase.Refresh();

        }

        public static void CreatePartialClass<T>(Type type) where T : Attribute
        {
            //ファイルの作成
            string filePath = Application.dataPath + $"/EZFrameWork/RealTime/RPCClasses/{type.Name}RPCPartial.cs";
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            using (writer)
            {
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using UnityEngine;");


                writer.WriteLine($"public partial class {type.Name}");
                writer.WriteLine("{");

                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo fInfo in fields)
                {
                    Attribute att = fInfo.GetCustomAttribute<T>();
                    if (att == null)
                        continue;

                    string first = fInfo.Name.Substring(0, 1).ToUpper();
                    string body = fInfo.Name.Substring(1).ToLower();
                    string propName = first + body;

                    //プロパティ作成
                    writer.WriteLine($"\tpublic {fInfo.FieldType} Sync{propName} {{");
                    writer.WriteLine($"\t\tget{{ return {fInfo.Name};}}");
                    writer.WriteLine($"\t\tset{{ Send{propName}PropertySync(value);}}");
                    writer.WriteLine($"\t\t}}\n");

                    //送信用関数作成
                    writer.WriteLine($"\tpublic void Send{propName}PropertySync ({fInfo.FieldType} value) {{");
                    //writer.WriteLine($"\t\tobject[] parameters = new object[]{{value}};");
                    writer.WriteLine($"\t\tstring propName = \"Receive{propName}PropertySync\";");
                    writer.WriteLine($"\t\tRTNetworkClient.Instance.SendGameEvent(GameEventCode.PROP_SYNC, networkInstanceId, propName, value);");
                    writer.WriteLine($"\t}}\n");

                    //受け取り用関数作成
                    writer.WriteLine($"\tpublic void Receive{propName}PropertySync ({fInfo.FieldType} value) {{");
                    writer.WriteLine($"\t\t{fInfo.Name} = value;");
                    writer.WriteLine($"\t\tOn{propName}ValueChanged ();");
                    writer.WriteLine($"\t}}\n");

                    //イベントpartial関数
                    writer.WriteLine($"\tpartial void On{propName}ValueChanged ();");
                }


                writer.WriteLine("}");

            }
        }

        static Dictionary<Type, List<FieldInfo>> GetAllClassesOfAttribute<T>() where T : Attribute
        {

            Assembly asm = Assembly.Load("Assembly-CSharp");

            Dictionary<Type, List<FieldInfo>> buffer = new Dictionary<Type, List<FieldInfo>>();
            Type[] types = asm.GetTypes();
            foreach (Type type in types)
            {
                //指定型の属性を持つ関数があるかどうか、なければいらない
                //Attribute attribu = type.GetCustomAttribute<T>();
                List<FieldInfo> fieldInfos = new List<FieldInfo>();
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo fInfo in fields)
                {
                    Attribute att = fInfo.GetCustomAttribute<T>();
                    if (att != null)
                    {
                        fieldInfos.Add(fInfo);
                        //buffer.Add(type, att);
                    }
                }

                //指定型の属性を持つ関数が存在しない、このクラスはスキップ
                if (fieldInfos.Count == 0)
                    continue;

                buffer.Add(type, fieldInfos);

            }
            return buffer;
        }

    }
}

