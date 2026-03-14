using UnityEngine;
using System.IO;
public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController =  FindFirstObjectByType<InventoryController>();
        hotbarController =  FindFirstObjectByType<HotbarController>();
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            InvSaveData = inventoryController.GetInventoryItems(),
            HotbarSaveData = hotbarController.GetHotbarItems()
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            inventoryController.SetInventoryItems(saveData.InvSaveData);
            hotbarController.SetHotbarItems(saveData.HotbarSaveData);
        }
        else
        {
            SaveGame();
        }
    }
}
