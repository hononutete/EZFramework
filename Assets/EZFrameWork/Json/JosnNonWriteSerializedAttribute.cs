using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EZFramework.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class JosnNonWriteSerializedAttribute : Attribute
    {
    }
}
