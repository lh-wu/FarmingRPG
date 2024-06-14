using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehavior<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    public List<InventoryItem>[] inventoryLists;

    [HideInInspector]
    public int[] inventoryListCapacityArray; 

    [SerializeField] private SO_ItemList itemList=null;



    
    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();
        CreateItemDetailsDictionary();

    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.Count];
        inventoryListCapacityArray = new int[(int)InventoryLocation.Count];

        for (int i=0;i< (int)InventoryLocation.Count; ++i)
        {
            inventoryLists[i] = new List<InventoryItem>();
            
        }
        inventoryListCapacityArray[(int)InventoryLocation.Player] = Settings.playerInitialInventoryCapacity;


    }


    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach(ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails ans;
        if (itemDetailsDictionary.TryGetValue(itemCode, out ans))
        {
            return ans;
        }
        return null;
    }

    public void Additem(InventoryLocation inventoryLocation, Item item, GameObject gameObject)
    {
        Additem(inventoryLocation, item);
        Destroy(gameObject);
    }


    public void Additem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryItems, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryItems, itemCode);
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

    }

    private void AddItemAtPosition(List<InventoryItem> inventoryItems, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryItems.Add(inventoryItem);

        DebugPrintInventoryList(inventoryItems);
    }


    private void AddItemAtPosition(List<InventoryItem> inventoryItems, int itemCode, int itemPosition)
    {
        int currentQuantity = inventoryItems[itemPosition].itemQuantity;

        // 由于itemPosition是struct类型(值类型),不能使用++inventoryItems[itemPosition].itemQuantity;
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = currentQuantity+1;
        inventoryItems[itemPosition] = inventoryItem;


        DebugPrintInventoryList(inventoryItems);
    }

    private int FindItemInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];
        for(int i = 0; i < inventoryItems.Count; ++i)
        {
            if (inventoryItems[i].itemCode == itemCode) { return i; }
        }
        return -1;
    }



    private void DebugPrintInventoryList(List<InventoryItem> inventoryItems)
    {
        foreach(var inventoryItem in inventoryItems)
        {
            Debug.Log("ItemCode: " + inventoryItem.itemCode + ", ItemDescription: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + ", Nums: " + inventoryItem.itemQuantity);
        }
        Debug.Log("*****************************************************************");
    }

}
