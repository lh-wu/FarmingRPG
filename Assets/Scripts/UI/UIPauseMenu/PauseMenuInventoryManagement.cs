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
        // ÿ������ʱ��������Ϸ�ı������˵��ı���
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
    /// ����ͣ�˵��ı���ʱ��������Ϸ�ڱ���������������
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
                // ����ã���ͣ�˵��ϵģ���Ʒ������item������ʾ��������sprite
                if (inventoryManagementSlot[i].itemDetails!= null)
                {
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }
            }
        }
    }

    /// <summary>
    /// ��ʼ����ͣ�˵�����״̬�����ø���Ϊ�գ������ø���Ϊ��ɫ
    /// </summary>
    private void InitialiseInventoryManagementSlots()
    {
        //�������и���Ϊ��ʼ״̬�����ã���Ϊ�գ�
        for(int i = 0; i < Settings.playerMaxInventoryCapacity; ++i)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }
        // ����ʵ�ʵı�����С����󱳰���С������ʱ�����õĸ��ӻ�ɫ�ڱ�
        for(int i = InventoryManager.Instance.inventoryListCapacityArray[(int)InventoryLocation.Player]; i < Settings.playerMaxInventoryCapacity; ++i)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }

    }

}
