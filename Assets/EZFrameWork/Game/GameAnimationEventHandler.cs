using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EZFramework.Game
{

    public class GameAnimationEventHandler : MonoBehaviour
    {
        public Action<string> onEventTriggered;

        public void OnEventTriggered(string arg)
        {
            if (onEventTriggered != null)
                onEventTriggered(arg);
        }


    }
}
