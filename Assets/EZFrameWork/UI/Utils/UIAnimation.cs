using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EZFramework.UI
{

    [RequireComponent(typeof(Animator))]
    public class UIAnimation : MonoBehaviour
    {
        public string KEY_SHOW = "show";
        public string KEY_HIDE = "hide";

        Animator anim;

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Show()
        {
            anim.SetTrigger(KEY_SHOW);
        }

        public void Hide()
        {
            anim.SetTrigger(KEY_HIDE);
        }

    }
}