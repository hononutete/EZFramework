using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EZFrameWorkSettings", menuName = "EZFrameWorkSettings/SettingFile", order = 1)]
public class EZFrameWorkSettings : ScriptableObject
{
    [SerializeField]
    public string MasterClassOutputPath = "Scripts/Models/";

    [SerializeField]
    public string MasterCsvInputPath = "LocalResources/masterdata/csv/";

    [SerializeField]
    public string AddressableMasterAssetOutputPath = "common/";
}
