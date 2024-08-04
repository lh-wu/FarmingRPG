using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isInventoryBarPositionBottom = true;

    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;
    public GameObject inventoryBarDraggedItem;

    public bool IsInventoryBarPositionBottom
    {
        set { isInventoryBarPositionBottom = value; }
        get { return isInventoryBarPositionBottom; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }


    /// <summary>
    /// 根据player相对于相机的位置，更改物品栏在界面顶部还是底部
    /// </summary>
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewPointPosition = PlayerController.Instance.GetPlayerViewPosition();
        if(playerViewPointPosition.y>0.3&& IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);
            IsInventoryBarPositionBottom = true;
        }
        else if(playerViewPointPosition.y <= 0.3 && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);
            IsInventoryBarPositionBottom = false;
        }
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdated;
    }


    /// <summary>
    /// 首先判断更新的背包是否为玩家背包，
    /// 是->清空SLot，根据inventoryList顺序为inventorySlots赋值
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="inventoryList"></param>
    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.Player)
        {
            ClearInventorySlots();
            if(inventorySlots.Length>0 && inventoryList.Count > 0)
            {
                for(int i=0;i< inventorySlots.Length; ++i)
                {
                    if(i< inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
                        if (itemDetails != null)
                        {
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightInventorySlots(i);
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; ++i)
            {
                inventorySlots[i].itemQuantity = 0;
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                SetHighlightInventorySlots(i);
            }
        }
    }


    /// <summary>
    /// 遍历主界面背包的所有格子(slot)，把其设置为未选中状态，并将slot的highlight的贴图颜色设置为透明
    /// 并调用InventoryManager方法ClearSelectInventoryItem
    /// </summary>
    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; ++i)
            {
                if(inventorySlots[i].isSelected == true)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    InventoryManager.Instance.ClearSelectInventoryItem(InventoryLocation.Player);
                }
            }
        }
    }


    public void SetHighlightInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; ++i)
            {
                SetHighlightInventorySlots(i);
            }
        }
    }


    /// <summary>
    /// 检查该slot的isSelected属性，并将其Highlight框设置为原来的颜色，并调用InventoryManager方法SetSelectInventoryItem
    /// </summary>
    /// <param name="i"></param>
    public void SetHighlightInventorySlots(int i)
    {
        if(inventorySlots.Length>0 && inventorySlots[i].itemDetails != null)
        {
            if (inventorySlots[i].isSelected)
            {
                inventorySlots[i].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);
                InventoryManager.Instance.SetSelectInventoryItem(InventoryLocation.Player, inventorySlots[i].itemDetails.itemCode);
            }
        }
    }

    public void DestoryCurrentlyDraggedItems()
    {
        for(int i = 0;i<inventorySlots.Length; ++i)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for(int i = 0; i < inventorySlots.Length; ++i)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }


}
