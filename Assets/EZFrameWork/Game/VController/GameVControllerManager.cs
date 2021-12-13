using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using EZFramework.Util;

namespace EZFramework.Game
{

    /// <summary>
    /// 仮想コントローラーのクラスを定義する。リフレクションでこの文字列からクラスを生成するため、クラス名と同じ名前で定義すること
    /// </summary>
    public enum EGameVControllerType
    {
        UI, ACTION, HAND, BUCKET_PLACEMENT
    }

    public class GameVControllerManager : SingletonMonobehaviour<GameVControllerManager>
    {
        Dictionary<int, GameVController> vControllers = new Dictionary<int, GameVController>();

        public void Init() { }

        public T GetOrCreate<T>(int eVControllerType) where T : GameVController
        {
            T c = Get<T>(eVControllerType);
            if (c == null)
                c = Create<T>(eVControllerType);
            return c;
        }

        public T Create<T>(int eVControllerType) where T : GameVController
        {
            //同じコントローラーは共存できない
            if (vControllers.Select((arg) => arg.Value.VControllerType).Contains(eVControllerType))
                return null;

            //共存できるかどうか
            foreach (GameVController vc in vControllers.Values)
            {
                if (vc.DoConflict(eVControllerType))
                    return null;
            }

            //TODO: クラス属性を使ってもいいかもしれない
            //リフレクションにより列挙子の文字列を使ってクラス生成
            Type type = typeof(T);// Type.GetType(eVControllerType.ToString());
            GameVController vController = Activator.CreateInstance(type) as GameVController;

            //初期化
            vController.SetEVControllerType(eVControllerType);
            vController.Init();

            //キャッシュ
            if (vController != null)
                vControllers.Add(eVControllerType, vController);

            return vController as T;
        }

        public T Get<T>(int eVControllerType) where T : GameVController
        {
            if (vControllers.ContainsKey(eVControllerType))
                return vControllers[eVControllerType] as T;
            return null;
        }

        public void Remove(int eVControllerType)
        {
            if (vControllers.ContainsKey(eVControllerType))
            {
                vControllers[eVControllerType].OnDestroy();
                vControllers.Remove(eVControllerType);
            }
        }

        public override void Dispose()
        {
            foreach (GameVController vc in vControllers.Values)
                vc.Destroy();
            vControllers.Clear();
            base.Dispose();
        }

        public void Update()
        {
            foreach (GameVController vc in vControllers.Values)
            {
                if (vc.isActive)
                    vc.Update();
            }
        }

    }
}
