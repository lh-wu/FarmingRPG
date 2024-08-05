using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlot = null;

    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameobject;


    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += PopulatePlayerInventory;
        // 每次启用时，根据游戏的背包填充菜单的背包
        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.Player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameobject();
    }

    public void DestroyInventoryTextBoxGameobject()
    {
        if(inventoryTextBoxGameobject!= null )
        {
            Destroy(inventoryTextBoxGameobject);
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for(int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player].Count; ++i)
        {
            if (inventoryManagementSlot[i].draggedItem!= null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }

    /// <summary>
    /// 打开暂停菜单的背包时，根据游戏内背包的情况进行填充
    /// </summary>
    private void PopulatePlayerInventory(InventoryLocation inventoryLocation,List<InventoryItem> playerInventoryList)
    {
        if(inventoryLocation == InventoryLocation.Player)
        {
            InitialiseInventoryManagementSlots();
            for(int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player].Count; ++i)
            {
                inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(playerInventoryList[i].itemCode);
                inventoryManagementSlot[i].itemQuantity = playerInventoryList[i].itemQuantity;
                // 如果该（暂停菜单上的）物品栏存在item，则显示其数量和sprite
                if (inventoryManagementSlot[i].itemDetails!= null)
                {
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 初始化暂停菜单格子状态。可用格子为空，不可用格子为灰色
    /// </summary>
    private void InitialiseInventoryManagementSlots()
    {
        //设置所有格子为初始状态（可用，且为空）
        for(int i = 0; i < Settings.playerMaxInventoryCapacity; ++i)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }
        // 根据实际的背包大小和最大背包大小，将暂时不可用的格子灰色遮蔽
        for(int i = InventoryManager.Instance.inventoryListCapacityArray[(int)InventoryLocation.Player]; i < Settings.playerMaxInventoryCapacity; ++i)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }

    }

}
