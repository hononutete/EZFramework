namespace EZFramework.Game
{
    interface IVControllerConfliction
    {
        bool DoConflict();
    }

    /// <summary>
    /// バトルで使う仮想コントローラー。
    /// コントローラー十字キーの前方向を押したら、前に移動する、キーボードでは「w」を押したら、前に移動する。
    /// このような意味づけを行う。つまり仮想コントローラーというものは
    /// ・前に移動
    /// ・横に移動
    /// ・銃を撃つ
    /// など入力に基づいたそのゲームに必要なアクションを羅列したものだと考えられる
    /// </summary>
    public abstract class GameVController
    {
        public void SetEVControllerType(int eVControllerType)
        {
            VControllerType = eVControllerType;
        }
        public virtual void Init() { }
        public virtual void OnDestroy() { }
        public virtual void Update() { }
        public int VControllerType { get; private set; }
        public bool DoConflict(int eVControllerType)
        {
            return false;
        }
        public void Destroy() => OnDestroy();
        protected int layerMask;
        public bool isActive = true;
    }
}