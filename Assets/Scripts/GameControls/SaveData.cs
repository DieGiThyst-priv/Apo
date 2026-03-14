using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public List<InventorySaveData> InvSaveData;
     public List<InventorySaveData> HotbarSaveData;
}
