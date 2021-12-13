

namespace EZFramework.AVG
{

    [System.Serializable]
    [MasterModel()]
    public partial class MAvgScenarioCut
    {
        /// <summary>
        /// フィールド名：ID
        /// id
        /// id
        /// </summary>
        public int ID;
        /// <summary>
        /// フィールド名：ScenarioId
        /// </summary>
        public int ScenarioId;
        /// <summary>
        /// フィールド名：NextConditionType
        /// 0:None
        /// 1:Force
        /// 2:Tap
        /// </summary>
        public int NextConditionType;
        /// <summary>
        /// フィールド名：NextConditionArg1
        /// </summary>
        public int NextConditionArg1;
        /// <summary>
        /// フィールド名：NextConditionArg2
        /// </summary>
        public int NextConditionArg2;
        /// <summary>
        /// フィールド名：NextId
        /// </summary>
        public int NextId;
        /// <summary>
        /// フィールド名：TextKey
        /// </summary>
        public string TextKey;
        /// <summary>
        /// フィールド名：FrameType
        /// </summary>
        public string FrameType;
        /// <summary>
        /// フィールド名：CharacterResourceName
        /// </summary>
        public string CharacterResourceName;
    }
}