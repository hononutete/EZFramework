using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Util
{
    public class Scheduler
    {
        float interval = -1;
        float timeStamp;

        public void SetInterval(float interval)
        {
            this.interval = interval;
        }

        public bool Check()
        {
            if (timeStamp == 0)
                timeStamp = Time.time;

            //0以下では常にfalse
            if (interval < 0)
                return false;

            if (Time.time - timeStamp > interval)
            {
                timeStamp = Time.time;
                return true;
            }
            return false;
        }

    }
}
