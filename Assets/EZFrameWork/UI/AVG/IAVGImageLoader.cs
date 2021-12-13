using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace EZFramework.AVG
{
    public interface IAVGImageLoader
    {
        public UniTask<Sprite> LoadCharacterImage(string path);

        public UniTask<Sprite> LoadFrameImage(string path);
    }
}
