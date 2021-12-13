using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public class Language
    {
        ILanguageTranslator languageTranslator;

        public void SetLanguageTranslator(ILanguageTranslator languageTranslator)
        {
            this.languageTranslator = languageTranslator;
        }

        public string GetText(string key)
        {
            if (languageTranslator != null)
            {
                return languageTranslator.GetText(key);
            }
            else
            {
                Debug.Log("error : language translator null");
                return string.Empty;
            }

        }
    }
}
