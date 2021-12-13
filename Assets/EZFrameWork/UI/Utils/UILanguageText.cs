using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZFramework.Game;

namespace EZFramework.UI
{
    public class UILanguageText : MonoBehaviour
    {
        //TODO:エディター状でプルダウンでマスターのMLanugeageのキーを選択できるように変更
        public string languageKey;

        void Start()
        {
            Text text = GetComponent<Text>();
            if (text != null)
            {
                text.text = GetText(text.text);
            }

            TextMeshProUGUI textMeshPro = GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = GetText(textMeshPro.text);
            }

        }

        string GetText(string rawText)
        {
            if (languageKey == string.Empty)
                return "";

            if (rawText == string.Empty)
                return ServiceLocatorProvider.Instance.Current.Resolve<Language>().GetText(languageKey);
            else
            {
                return string.Format(rawText, ServiceLocatorProvider.Instance.Current.Resolve<Language>().GetText(languageKey));
            }
        }
    }
}
