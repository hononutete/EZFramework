using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework
{

    [CreateAssetMenu(fileName = "MaterialAddressableAssetReference", menuName = "ScriptableObjects/MaterialAddressableAssetReference", order = 1)]
    public class MaterialAddressableAssetReference : ScriptableObject
    {
        public List<Material> materials;
    }
}

