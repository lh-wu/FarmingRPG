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
        // 根据背包的数量（nventoryLocation.Count）创建对应数量的列表
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

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.BreakingTool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.ChoppingTool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.CollectingTool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            case ItemType.HoeingTool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.ReapingTool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.WateringTool:
                itemTypeDescription = Settings.WateringTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }


    /// <summary>
    /// 将item添加到inventoryLocation中，并destory gameobject
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    /// <param name="gameObject"></param>
    public void Additem(InventoryLocation inventoryLocation, Item item, GameObject gameObject)
    {
        Additem(inventoryLocation, item);
        Destroy(gameObject);
    }

    /// <summary>
    /// 仅将item添加到inventoryLocation中，不删除gameobject
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
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

        // DebugPrintInventoryList(inventoryItems);
    }

    /// <summary>
    /// 查找要删除的物品的位置，调用函数进行删除，并更新
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    internal void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    /// <summary>
    /// 从指定的背包中的指定位置，减少一个物品的量，被RemoveItem调用
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    /// <param name="itemPosition"></param>
    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[itemPosition].itemQuantity-1;
        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[itemPosition] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(itemPosition);
        }

    }

    private void AddItemAtPosition(List<InventoryItem> inventoryItems, int itemCode, int itemPosition)
    {
        int currentQuantity = inventoryItems[itemPosition].itemQuantity;

        // 由于itemPosition是struct类型(值类型),不能使用++inventoryItems[itemPosition].itemQuantity;
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = currentQuantity+1;
        inventoryItems[itemPosition] = inventoryItem;


        // DebugPrintInventoryList(inventoryItems);
    }

    /// <summary>
    /// 根据itemCode检查inventoryLocation中是否已经存在该物品，并返回int类型的pos，没找到为-1
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    private int FindItemInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];
        for(int i = 0; i < inventoryItems.Count; ++i)
        {
            if (inventoryItems[i].itemCode == itemCode) { return i; }
        }
        return -1;
    }

    /// <summary>
    /// 交换inventoryLocation的背包的srcSlotID位置与dstSlotID的物品，不可与空的物体交换
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="srcSlotID"></param>
    /// <param name="dstSlotID"></param>
    public void SwapUIInventorySlot(InventoryLocation inventoryLocation, int srcSlotID, int dstSlotID)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];
        if (srcSlotID >= inventoryItems.Count || dstSlotID >= inventoryItems.Count)
        {
            return;
        }
        InventoryItem srcInventoryItem = inventoryItems[srcSlotID];
        InventoryItem dstInventoryItem = inventoryItems[dstSlotID];
        inventoryItems[srcSlotID] = dstInventoryItem;
        inventoryItems[dstSlotID] = srcInventoryItem;
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);


    }

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryItems)
    //{
    //    foreach(var inventoryItem in inventoryItems)
    //    {
    //        Debug.Log("ItemCode: " + inventoryItem.itemCode + ", ItemDescription: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + ", Nums: " + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("*****************************************************************");
    //}

}
