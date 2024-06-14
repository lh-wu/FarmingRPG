
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            ItemDetails itemDetails;
            itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);
            if(itemDetails.canPickUp == true)
            {
                InventoryManager.Instance.Additem(InventoryLocation.Player, item, collision.gameObject);
            }
            
        }
    }
}
