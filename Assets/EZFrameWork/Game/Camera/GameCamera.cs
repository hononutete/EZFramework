using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using EZFramework.Util;

namespace EZFramework.Game
{
    public class GameCamera : SingletonMonobehaviour<GameCamera>
    {
        /// <summary>
        /// 視点のターゲット
        /// </summary>
        Transform eyeTarget;

        /// <summary>
        /// 対象を目視するターゲット
        /// </summary>
        Transform lookAtTarget;

        void Update()
        {
            if (lookAtTarget != null)
                transform.LookAt(lookAtTarget);

            if (eyeTarget != null)
            {
                transform.position = eyeTarget.position;
                transform.eulerAngles = eyeTarget.eulerAngles;
            }
        }

        public void ReleaseEyeTarget()
        {
            SetEyeTarget(null);
        }

        public void SetEyeTarget(Transform eyeTarget)
        {
            this.eyeTarget = eyeTarget;
        }

        public void ClearLookAtTarget()
        {
            SetLookAtTarget(null);
        }

        public void SetLookAtTarget(Transform lookAtTarget)
        {
            this.lookAtTarget = lookAtTarget;
        }
        public void Move(Vector3 to, float duration, Action onComplete = null)
        {

            transform.DOMove(to, duration).OnComplete(() => { if (onComplete != null) onComplete(); });
        }

        public void RotateQuaternion(Quaternion to, float duration, Action onComplete = null)
        {
            transform.DORotateQuaternion(to, duration).OnComplete(() => { if (onComplete != null) onComplete(); });
        }

        public void LookAt(Transform target, float duration, Action onComplete = null)
        {
            transform.DOLookAt(target.position, duration).OnComplete(() => { if (onComplete != null) onComplete(); });
        }
    }
}