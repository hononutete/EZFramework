using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.AVG
{
    /// <summary>
    /// シナリオのローダー。それぞれでカスタムして
    /// </summary>
    public class AVGScenarioLoader
    {
        public virtual List<MAvgScenarioCut> LoadScenarioCut()
        {
            return new List<MAvgScenarioCut>();
        }
    }

    public class MCGAVGScenarioLoader : AVGScenarioLoader
    {
        public int scenarioId;

        public override List<MAvgScenarioCut> LoadScenarioCut()
        {
            return new List<MAvgScenarioCut>();
        }
    }
}

