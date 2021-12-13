using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Util
{
    public class TextStream
    {
        public float textSpeed;
        int index = 0;
        string text;

        public void Write(string text)
        {
            this.text = text;
            index = 0;
        }

        public string ReadNext()
        {
            string str = string.Empty;

            if (index < text.Length)
            {
                str = text[index].ToString();
                index++;
            }
            return str;
        }

    }
}
