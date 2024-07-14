using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorIsEnable;
    public bool CursorIsEnable { get { return _cursorIsEnable; } set { _cursorIsEnable = value; } }

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get { return _cursorPositionIsValid; } set { _cursorPositionIsValid = value; } }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get { return _selectedItemType; } set { _selectedItemType = value; } }

    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get { return _itemUseRadius; } set { _itemUseRadius = value; } }


    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        // 获取鼠标位置
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();
        //设置cursor的sprite
        SetCursorValidity(cursorWorldPosition, PlayerController.Instance.GetPlayerCentrePosition());
        //设置cursor的位置
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();
        // 去除四个角点的矩形
        if(
            cursorPosition.x>(playerPosition.x+ItemUseRadius/2f)&&cursorPosition.y>(playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            )
        {
            SetCursorToInvalid();
            return;
        }

        if(Mathf.Abs(cursorPosition.x-playerPosition.x)>ItemUseRadius|| Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        switch (itemDetails.itemType)
        {
            case ItemType.WateringTool:
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
            case ItemType.HoeingTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.none:
                break;
            case ItemType.count:
                break;
            default:break;
        }

    }

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition,ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.ReapingTool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
            default:
                return false;
        }
    }
    /// <summary>
    /// 先获取鼠标位置的ItemList，然后再查看是否存在可reap的item
    /// </summary>
    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();
        if(HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            if (itemList.Count != 0)
            {
                foreach(Item item in itemList)
                {
                    if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.ReapableScenary)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        gridCursor.DisableCursor();
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;
        gridCursor.EnableCursor();
    }



    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }


    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnable = true;
    }
}
