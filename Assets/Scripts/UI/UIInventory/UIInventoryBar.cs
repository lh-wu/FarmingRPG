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
    /// ����player����������λ�ã�������Ʒ���ڽ��涥�����ǵײ�
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
    /// �����жϸ��µı����Ƿ�Ϊ��ұ�����
    /// ��->���SLot������inventoryList˳��ΪinventorySlots��ֵ
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
    /// ���������汳�������и���(slot)����������Ϊδѡ��״̬������slot��highlight����ͼ��ɫ����Ϊ͸��
    /// ������InventoryManager����ClearSelectInventoryItem
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
    /// ����slot��isSelected���ԣ�������Highlight������Ϊԭ������ɫ��������InventoryManager����SetSelectInventoryItem
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
