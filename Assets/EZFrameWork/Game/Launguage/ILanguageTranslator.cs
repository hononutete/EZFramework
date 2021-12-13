using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public interface ILanguageTranslator
    {
        string GetText(string key);
    }
}
