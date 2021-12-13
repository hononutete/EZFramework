using EZFramework.Util;

namespace EZFramework.Game
{
    public enum EInputType
    {
        TOUCH = Bit.BIT_FLAG_1,
        KEYBOARD_MOUSE = Bit.BIT_FLAG_2,
        GYRO_ROTATION = Bit.BIT_FLAG_4,
        GYRO_ACCELERATION = Bit.BIT_FLAG_8
    }

    public class GameInputManager : SingletonMonobehaviour<GameInputManager>
    {
        /// <summary>
        /// 有効化されている入力種類のビットフラグ
        /// </summary>
        EInputType inputType;

        // 入力が共存する可能性があるから別々の変数を定義
        public GameInputTouch InputTouch { get; private set; }
        public GameInputKeyboardMouse InputKeyboardMouse { get; private set; }
        public GameInputGyroRotation InputGyroRotation { get; private set; }
        public GameInputGyroAcceleration InputGyroAcceleration { get; private set; }

        public void Init(EInputType inputType)
        {
            this.inputType = inputType;

            if (((int)inputType & (int)EInputType.TOUCH) != 0)
            {
                InputTouch = gameObject.AddComponent<GameInputTouch>();
            }
            if (((int)inputType & (int)EInputType.KEYBOARD_MOUSE) != 0)
            {
                InputKeyboardMouse = gameObject.AddComponent<GameInputKeyboardMouse>();
            }
            if (((int)inputType & (int)EInputType.GYRO_ROTATION) != 0)
            {
                InputGyroRotation = gameObject.AddComponent<GameInputGyroRotation>();
            }
            if (((int)inputType & (int)EInputType.GYRO_ACCELERATION) != 0)
            {
                InputGyroAcceleration = gameObject.AddComponent<GameInputGyroAcceleration>();
            }

            //inputのコールバックにイベントをセット
            SetInputCallback();
        }

        public override void Dispose()
        {
            if (InputTouch != null)
            {
                Destroy(InputTouch);
                InputTouch = null;
            }
            if (InputKeyboardMouse != null)
            {
                Destroy(InputKeyboardMouse);
                InputKeyboardMouse = null;
            }
            if (InputGyroRotation != null)
            {
                Destroy(InputGyroRotation);
                InputGyroRotation = null;
            }
            if (InputGyroAcceleration != null)
            {
                Destroy(InputGyroAcceleration);
                InputGyroAcceleration = null;
            }

            base.Dispose();
        }

        protected virtual void SetInputCallback() { }
    }
}
