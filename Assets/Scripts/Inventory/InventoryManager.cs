using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehavior<InventoryManager>, ISaveable
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary; //����ΪitemCode����Ʒ�����ֵ�
    public List<InventoryItem>[] inventoryLists;                //����

    [HideInInspector]
    public int[] inventoryListCapacityArray;            // ���ڱ�ʶ����������

    [SerializeField] private SO_ItemList itemList=null; // ��ȡSO�ļ� 

    private UIInventoryBar inventoryBar;


    private int[] selectedInventoryItem;                // ���ڱ�ʶÿһ����������ѡ�����ĸ�item���洢��ΪitemCode

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get {  return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; }set { _gameObjectSave = value; } }

    
    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();
        CreateItemDetailsDictionary();
        selectedInventoryItem = new int[(int)InventoryLocation.Count];
        for(int i = 0; i < (int)InventoryLocation.Count; ++i)
        {
            selectedInventoryItem[i] = -1;
        }

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();

    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.Count];
        inventoryListCapacityArray = new int[(int)InventoryLocation.Count];
        // ���ݱ�����������nventoryLocation.Count��������Ӧ�������б�
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
    /// ��item��ӵ�inventoryLocation�У���destory gameobject
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    /// <param name="gameObject"></param>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObject)
    {
        AddItem(inventoryLocation, item);
        Destroy(gameObject);
    }

    /// <summary>
    /// ����item��ӵ�inventoryLocation�У���ɾ��gameobject
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
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
    /// ����ֱ����inventoryLocaiton�ı�������һ��item
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    /// <summary>
    /// ����Ҫɾ������Ʒ��λ�ã����ú�������ɾ����������
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
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
    /// ��ָ���ı����е�ָ��λ�ã�����һ����Ʒ��������RemoveItem����
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

        // ����itemPosition��struct����(ֵ����),����ʹ��++inventoryItems[itemPosition].itemQuantity;
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = currentQuantity+1;
        inventoryItems[itemPosition] = inventoryItem;


        // DebugPrintInventoryList(inventoryItems);
    }

    /// <summary>
    /// ����itemCode���inventoryLocation���Ƿ��Ѿ����ڸ���Ʒ��������int���͵�pos��û�ҵ�Ϊ-1
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public int FindItemInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];
        for(int i = 0; i < inventoryItems.Count; ++i)
        {
            if (inventoryItems[i].itemCode == itemCode) { return i; }
        }
        return -1;
    }

    /// <summary>
    /// ����inventoryLocation�ı�����srcSlotIDλ����dstSlotID����Ʒ��������յ����彻��
    /// </summary>
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
    
    public void SetSelectInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    /// <summary>
    /// ����Ӧ����ѡ�е�idxȡ��
    /// </summary>
    /// <param name="inventoryLocation"></param>
    public void ClearSelectInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }


    /// <summary>
    /// ����inventoryLocation����ѡ�е������itemCode
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }


    /// <summary>
    /// ����inventoryLocation����ѡ�е������itemDetails
    /// </summary>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);
        if(itemCode == -1) { return null; }
        else { return GetItemDetails(itemCode); }
    }




    //private void DebugPrintInventoryList(List<InventoryItem> inventoryItems)
    //{
    //    foreach(var inventoryItem in inventoryItems)
    //    {
    //        Debug.Log("ItemCode: " + inventoryItem.itemCode + ", ItemDescription: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + ", Nums: " + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("*****************************************************************");
    //}


    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    public GameObjectSave ISaveableSave()
    {
        // ��ձ�������ԭ�����������
        SceneSave sceneSave = new SceneSave();
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        // ���������ݶ���������
        sceneSave.listInvItemArray = inventoryLists;
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityArray);
        // ���浽��������
        GameObjectSave.sceneData.Add(Settings.PersistentScene,sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                // �ָ���Ʒ���е�item
                if (sceneSave.listInvItemArray != null)
                {
                    inventoryLists = sceneSave.listInvItemArray;
                    // �ָ�ÿһ����Ʒ��
                    for(int i = 0; i < (int)InventoryLocation.Count; ++i)
                    {
                        EventHandler.CallInventoryUpdateEvent((InventoryLocation)i, inventoryLists[i]);
                    }
                    PlayerController.Instance.ClearCarriedItem();
                    inventoryBar.ClearHighlightOnInventorySlots();
                }
                // �ָ�ÿ����Ʒ������������
                if(sceneSave.intArrayDictionary != null&&sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray",out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityArray = inventoryCapacityArray;
                }
            }
        }
    }





    public void ISaveableStoreScene(string sceneName) { }
    public void ISaveableRestoreScene(string sceneName) { }


}
