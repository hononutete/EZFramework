using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace EZFramework.CSV
{
    public class CSVParser
    {
        public static Dictionary<string, T> ParseAsDictionary<T>(string filePath) where T : new()
        {
            Dictionary<string, T> table = new Dictionary<string, T>();

            //string型のテーブルを作成
            List<List<string>> rawTable = new List<List<string>>();

            //文字コードのチェックを入れるべきか
            StreamReader sr = new StreamReader(filePath, Encoding.UTF8);
            try
            {
                while (sr.EndOfStream == false)
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        //カンマ区切りの正規表現、””を覗く
                        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        string[] elem = reg.Split(line);

                        rawTable.Add(elem.ToList());
                    }
                }
            }
            finally
            {
                sr.Close();
            }

            //string型のテーブルを指定された型に変換
            List<string> header = rawTable[0];
            Dictionary<string, FieldInfo> fieldInfos = new Dictionary<string, FieldInfo>();

            //ヘッダーをキャッシュ
            for (int i = 0; i < header.Count; i++)
            {
                FieldInfo fieldInfo = typeof(T).GetField(header[i].Replace("\"", ""));

                fieldInfos.Add(header[i], fieldInfo);
            }

            //指定された型のインスタンスを作成しテーブルに挿入
            for (int i = 1; i < rawTable.Count; i++)
            {
                T column = new T();
                for (int j = 0; j < header.Count; j++)
                {
                    //型変換を行う、サポートする型はbool string int float

                    if (fieldInfos[header[j]].FieldType == typeof(int))
                    {
                        string str = rawTable[i][j] == "" ? "0" : rawTable[i][j];
                        fieldInfos[header[j]].SetValue(column, Convert.ToInt32(str));
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(float))
                    {
                        fieldInfos[header[j]].SetValue(column, Convert.ToSingle(rawTable[i][j]));
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(bool))
                    {
                        fieldInfos[header[j]].SetValue(column, rawTable[i][j] == "t");
                    }
                    else
                        fieldInfos[header[j]].SetValue(column, rawTable[i][j]);
                }
                table.Add(rawTable[i][0], column);
            }

            return table;

        }

        public static List<T> Parse<T>(string filePath) where T : new()
        {
            //ファイルの存在を確認
            if (!File.Exists(filePath))
            {
                Debug.Log("cannot find : " + filePath);
                return new List<T>();
            }

            List<T> table = new List<T>();

            //string型のテーブルを作成
            List<List<string>> rawTable = new List<List<string>>();

            //文字コードのチェックを入れるべきか
            StreamReader sr = new StreamReader(filePath, Encoding.UTF8);
            try
            {
                while (sr.EndOfStream == false)
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        //カンマ区切りの正規表現、””を覗く
                        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        string[] elem = reg.Split(line);

                        rawTable.Add(elem.ToList());
                    }
                }
            }
            finally
            {
                sr.Close();
            }

            //string型のテーブルを指定された型に変換
            List<string> header = rawTable[0];
            Dictionary<string, FieldInfo> fieldInfos = new Dictionary<string, FieldInfo>();

            //ヘッダーをキャッシュ
            for (int i = 0; i < header.Count; i++)
            {
                FieldInfo fieldInfo = typeof(T).GetField(header[i].Replace("\"", ""));

                fieldInfos.Add(header[i], fieldInfo);
            }

            //指定された型のインスタンスを作成しテーブルに挿入
            for (int i = 1; i < rawTable.Count; i++)
            {
                T column = new T();
                for (int j = 0; j < header.Count; j++)
                {
                    //型変換を行う、サポートする型はbool string int float List<int>
                    if (fieldInfos[header[j]].FieldType == typeof(byte))
                    {
                        string str = rawTable[i][j] == "" ? "0" : rawTable[i][j];
                        fieldInfos[header[j]].SetValue(column, Convert.ToByte(str));
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(int))
                    {
                        string str = rawTable[i][j] == "" ? "0" : rawTable[i][j];
                        fieldInfos[header[j]].SetValue(column, Convert.ToInt32(str));
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(float))
                    {
                        string str = rawTable[i][j] == "" ? "0" : rawTable[i][j];
                        fieldInfos[header[j]].SetValue(column, Convert.ToSingle(str));
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(bool))
                    {
                        fieldInfos[header[j]].SetValue(column, rawTable[i][j] == "t");
                    }
                    else if (fieldInfos[header[j]].FieldType == typeof(List<int>))
                    {
                        fieldInfos[header[j]].SetValue(column, ToList(rawTable[i][j]));
                    }
                    //string
                    else
                    {

                        string rv = rawTable[i][j].Replace("\"", "");
                        fieldInfos[header[j]].SetValue(column, rv);
                    }
                }
                table.Add(column);
            }
            Debug.Log($"parse success!! filepath = {filePath}");
            return table;

        }

        static List<int> ToList(string data)
        {
            data = data.Replace("\"", "");
            string[] dataList = data.Split(',');
            return dataList.Select((arg) => Convert.ToInt32(arg)).ToList();
        }


    }
}
