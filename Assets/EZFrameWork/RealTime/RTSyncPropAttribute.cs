using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EZFramework.Realtime
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RTSyncPropAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RTSyncMethodAttribute : Attribute
    {

    }
}
