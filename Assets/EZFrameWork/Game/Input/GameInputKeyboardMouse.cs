using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game
{
    public enum EKeyboardButtonState
    {
        NONE, DOWN, UP, PRESSED
    }
    public class GameInputKeyboardMouse : GameInput
    {
        public event Action<Vector3> onClickBegun;
        public event Action<Vector3> onClickEnded;
        public event Action<Vector3, Vector2> onDrag;
        public event Action<Vector3> onUpdate;

        public event Action<EKeyboardButtonState> onWKey;

        public event Action<EKeyboardButtonState> onAKey;

        public event Action<EKeyboardButtonState> onSKey;

        public event Action<EKeyboardButtonState> onDKey;

        public event Action<EKeyboardButtonState> onSpaceKey;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOnUGUI(Input.mousePosition))
                    return;

                if (onClickBegun != null)
                    onClickBegun(Input.mousePosition);

            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsPointerOnUGUI(Input.mousePosition))
                    return;

                if (onClickEnded != null)
                    onClickEnded(Input.mousePosition);

            }
            else if (Input.GetMouseButton(0))
            {
                if (IsPointerOnUGUI(Input.mousePosition))
                    return;

                if (onDrag != null)
                    onDrag(Input.mousePosition, Input.mouseScrollDelta);

            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (onWKey != null) onWKey(EKeyboardButtonState.DOWN);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                if (onWKey != null) onWKey(EKeyboardButtonState.PRESSED);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                if (onWKey != null) onWKey(EKeyboardButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (onAKey != null) onAKey(EKeyboardButtonState.DOWN);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (onAKey != null) onAKey(EKeyboardButtonState.PRESSED);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                if (onAKey != null) onAKey(EKeyboardButtonState.UP);
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                if (onSKey != null) onSKey(EKeyboardButtonState.DOWN);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (onSKey != null) onSKey(EKeyboardButtonState.PRESSED);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                if (onSKey != null) onSKey(EKeyboardButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (onDKey != null) onDKey(EKeyboardButtonState.DOWN);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (onDKey != null) onDKey(EKeyboardButtonState.PRESSED);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                if (onDKey != null) onDKey(EKeyboardButtonState.UP);
            }


            if (Input.GetKeyDown(KeyCode.Space))
                if (onSpaceKey != null) onSpaceKey(EKeyboardButtonState.DOWN);
            if (Input.GetKeyUp(KeyCode.Space))
                if (onSpaceKey != null) onSpaceKey(EKeyboardButtonState.UP);


            if (onUpdate != null)
                onUpdate(Input.mousePosition);
        }

        public bool IsKeyPressed(KeyCode key)
        {
            return Input.GetKey(key);
        }
    }
}
