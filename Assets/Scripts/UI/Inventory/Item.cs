using UnityEngine;
using UnityEngine.UI;
public class Item : Interactable
{
    public int ID;
    public string Name;

    public override void Interact(GameObject interactor)
    {
        InventoryController inventory = FindFirstObjectByType<InventoryController>();
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
        }

        bool itemAdded = inventory.AddItem(gameObject);

        if (itemAdded)
        {
            Destroy(gameObject);
        }
    }

    public virtual void UseItem()
    {
        Debug.Log("Using item" + Name);
        GameObject player = GameObject.FindWithTag("Player");
        if (this.Name == "Gun")
        {
            player.GetComponent<PlayerMovement>().EquipGun(true);
            Debug.Log("Equipped gun");   
        }
        else
        {
            player.GetComponent<PlayerMovement>().EquipGun(false);
            Debug.Log("Unequipped gun");
        }
    }
}
