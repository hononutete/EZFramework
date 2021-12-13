using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace EZFramework.Editor
{
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/AssetBundles/";
            string assetBundleResourceDirectory = "/AssetBundleResources/Resources/";

            string platformPath = "";
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                platformPath = "Android";
            }

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                platformPath = "iOS";
            }

            string dirPath = assetBundleDirectory + platformPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            RecursiveDirectoryCreate(Application.dataPath + assetBundleResourceDirectory);


            BuildPipeline.BuildAssetBundles(dirPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            //バージョンファイルを作成
            string assetBundlePlatform = "/AssetBundles/" + platformPath;
            CreateVersionFile(Application.dataPath + assetBundlePlatform, "versioninfo");

            //バージョンファイルアセットのアセットバンドルのビルド
            //Application.dataPath + assetBundlePlatform + AssetBundleConfig.VERSION_FILE_NAME
        }

        static void RecursiveDirectoryCreate(string dirPath)
        {
            //ディレクトリ内のファイルを取得
            string[] files = Directory.GetFiles(dirPath);

            //アセットファイルが存在した場合
            foreach (string file in files)
            {
                //.metaを覗く
                string ext = Path.GetExtension(file);
                if (ext == ".meta" || ext == ".DS_Store")
                    continue;

                //アセットにアセットバンドル名をセット、バリアントは使用しない
                string p = file.Replace(Application.dataPath, "");
                Debug.Log(p);
                AssetImporter importer = AssetImporter.GetAtPath("Assets" + p);
                string abname = p.Replace("/AssetBundleResources/Resources/", "");
                abname = abname.Replace(ext, "");
                importer.assetBundleName = abname;

                //アセットの場合、アセットバンドルをビルド

            }

            //ディレクトリだった場合再帰処理
            //ファイル構成からアセットバンドル名を決める。基本的には１アセット、１バンドルに対応
            //Resouce以下のディレクトリ構成を取得
            string[] directories = Directory.GetDirectories(dirPath);
            foreach (string dir in directories)
            {
                RecursiveDirectoryCreate(dir);
            }
        }

        /// <summary>
        /// バージョンファイルを作成
        /// </summary>
        static void CreateVersionFile(string path, string fileName)
        {
            //作成済みのアセットバンドル全ての情報を作成


            //scriptable objectアセットを作成
            BundleVersionInfo info = ScriptableObject.CreateInstance<BundleVersionInfo>();

            //情報セット
            info.versionInfos = new List<VersionInfo>();

            RecursiveVersionInfoCreate(path, path, info.versionInfos);

            AssetDatabase.CreateAsset(info, path + "/" + fileName);


        }

        static void RecursiveVersionInfoCreate(string basePath, string dirPath, List<VersionInfo> versionInfos)
        {
            //ディレクトリ内のファイルを取得
            string[] files = Directory.GetFiles(dirPath);

            //アセットファイルが存在した場合
            foreach (string file in files)
            {
                //.metaを覗く
                string ext = Path.GetExtension(file);
                if (ext == ".meta")
                    continue;

                string p = file.Replace(basePath, "");
                p = p.Replace(ext, "");

                VersionInfo versionInfo = new VersionInfo();
                versionInfo.name = p;
                //versionInfo.fileHash = 

                versionInfos.Add(versionInfo);

            }

            //ディレクトリだった場合再帰処理
            //ファイル構成からアセットバンドル名を決める。基本的には１アセット、１バンドルに対応
            //Resouce以下のディレクトリ構成を取得
            string[] directories = Directory.GetDirectories(dirPath);
            foreach (string dir in directories)
            {
                RecursiveVersionInfoCreate(basePath, dir, versionInfos);
            }
        }

    }
}