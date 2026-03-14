using UnityEngine;
using System.Collections.Generic;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDict;

    private void Awake()
    {
        itemDict = new Dictionary<int, GameObject>();
        for(int i=0; i<itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i+1;   
            }
         
        }

        foreach(Item item in itemPrefabs)
        {
            itemDict[item.ID] = item.gameObject;
        }
    }

    public GameObject getItemPrefab(int itemID)
    {
        itemDict.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            Debug.Log($"Item with ID {itemID} not found");
        }
        return prefab;
    }
}
