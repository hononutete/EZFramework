using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EZFramework
{

    [Serializable]
    [CreateAssetMenu(fileName = "UserModel", menuName = "ScriptableObjects/UserModel", order = 1)]
    public partial class UserModel : ScriptableObject
    {
        public string UserID;
        public string AccessToken;
        public int Permission;
        public string Platform;
        public string OnetimeToken;
    }
}
