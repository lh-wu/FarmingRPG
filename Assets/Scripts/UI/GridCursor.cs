using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;                      //��Ҫһ��image�����Ϊ��������image����ΪnoneҲ��
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get { return _cursorPositionIsValid; }set { _cursorPositionIsValid = value; } }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get {  return _itemUseGridRadius; } set {  _itemUseGridRadius = value; } }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get { return _selectedItemType; } set { _selectedItemType = value; } }

    private bool _cursorIsEnable;
    public bool CursorIsEnable {  get { return _cursorIsEnable; } set { _cursorIsEnable = value; } }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

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

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCursor();
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            SetCursorValidity(gridPosition,playerGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);
            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private Vector3Int GetGridPositionForCursor()
    {
        Vector3 cursorWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-mainCamera.transform.position.z));
        return grid.WorldToCell(cursorWorldPosition);
    }

    private Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(PlayerController.Instance.transform.position);
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();
        // ����ʹ�÷�Χ��gridCursor��Ϊ��ɫ
        if(Mathf.Abs(cursorGridPosition.x-playerGridPosition.x)>ItemUseGridRadius|| 
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius) 
        {
            SetCursorToInvalid();
            return;
        }
        // û��ѡ�е����壬gridCursor��Ϊ��ɫ
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid(); return;
                    }
                    break;
                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid(); return;
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count: 
                    break;
                default: 
                    break;
            } 
        }
        else
        {
            SetCursorToInvalid(); return;
        }
    }


    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// gridCursor����Ϊ��ɫ
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    /// <summary>
    /// gridCursor����Ϊ��ɫ
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnable = true;
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridSceenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridSceenPosition, cursorRectTransform, canvas);
    }

}
