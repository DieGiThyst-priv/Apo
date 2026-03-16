using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    public HotbarController hotbarController;
    [SerializeField] GameObject gun;
    

    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        hotbarController = GetComponent<HotbarController>();
        /*  for(int i=0; i< slotCount; i++)
         {
          Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
             if (i < itemPrefabs.Length)
             {
                 GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                 item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                 slot.currentItem = item;
             }
         } */
    }

    private void Awake()
    {
        this.StartWithGunInInventory();
    }

    void StartWithGunInInventory()
    {
        this.TryAddToPanel(hotbarController.hotbarPanel.transform, gun);
    }



    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData {ItemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });

            }
        }
        return invData;
    }

    public bool AddItem(GameObject itemPrefab)
    {
        if (TryAddToPanel(hotbarController.hotbarPanel.transform, itemPrefab))
            return true;

        if (TryAddToPanel(inventoryPanel.transform, itemPrefab))
            return true;

        Debug.Log("Inventory full");
        return false;
    }

    private bool TryAddToPanel(Transform panel, GameObject itemPrefab)
    {
        Debug.Log("Entry");
        foreach (Transform slotTransform in panel)
        {
            Debug.Log(slotTransform);
            Slot slot = slotTransform.GetComponent<Slot>();
            Debug.Log(slot.currentItem);
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;
                return true;
            }
        }

        return false;
    }

    public void SetInventoryItems(List<InventorySaveData> savedata)
    {
        //clear inventory panels
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i=0; i<slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        foreach(InventorySaveData data in savedata)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.getItemPrefab(data.ItemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }

}
