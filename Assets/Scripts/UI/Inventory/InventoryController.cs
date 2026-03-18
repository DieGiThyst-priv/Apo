using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public List<GameObject> itemPrefabs = new List<GameObject>();
    public HotbarController hotbarController;

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

        SubscribeToPanel(inventoryPanel.transform);
        SubscribeToPanel(hotbarController.hotbarPanel.transform);
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
        foreach (Transform slotTransform in panel)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

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

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
            Slot slot = slotObj.GetComponent<Slot>();

            slot.OnItemChanged += HandleItemChanged;
        }

        foreach (InventorySaveData data in savedata)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                slot.OnItemChanged += HandleItemChanged;

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

    void HandleItemChanged(Slot slot, GameObject oldItem, GameObject newItem)
    {
        if (oldItem != null)
        {
            if (!IsItemInAnySlot(oldItem))
            {
                itemPrefabs.Remove(oldItem);
            }
        }
        if (newItem != null)
        {
            if (!itemPrefabs.Contains(newItem))
            {
                itemPrefabs.Add(newItem);
            }
        }
    }

    bool IsItemInAnySlot(GameObject item)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot s = slotTransform.GetComponent<Slot>();
            if (s != null && s.currentItem == item)
                return true;
        }
        foreach (Transform slotTransform in hotbarController.hotbarPanel.transform)
        {
            Slot s = slotTransform.GetComponent<Slot>();
            if (s != null && s.currentItem == item)
                return true;
        }

        return false;
    }

    public void SubscribeToPanel(Transform panel)
    {
        foreach (Transform slotTransform in panel)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null)
            {
                slot.OnItemChanged += HandleItemChanged;
            }
        }
    }

}
