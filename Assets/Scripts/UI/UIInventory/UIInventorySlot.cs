using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Camera mainCamera;
    private GameObject draggedItem;
    private Transform parentItem;           //当物品被拖到场景后，应该将其归类到parentItem的子目录里面(Hierachy界面)
    private Canvas parentCanvas;
    private GridCursor gridCursor;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [HideInInspector] public bool isSelected = false;
    [SerializeField] private int slotID = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = GameObject.FindObjectOfType<GridCursor>();

    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchParentItem;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchParentItem;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }

    /// <summary>
    /// 在场景切换的时候更改parentItem为当前场景下的item（一个游戏object，该场景的所有item都被放在其中）
    /// </summary>
    private void SwitchParentItem()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            PlayerController.Instance.DisablePlayerInputAndResetMovenment();
            // 此处选用inventoryBar.transform，仅仅因为inventoryBar有canvas组件，才能使得拖拽可以正常显示
            // 该Instantiate函数将初始化的object当作inventoryBar的子物体
            // 该处初始化的位置马上会被OnEndDrag设置为跟随鼠标，所以初始位置不重要
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            SetSelectedItem();
        }
    }

    /// <summary>
    /// 拖拽的物体跟随鼠标位置
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            // 该if判断最终是否拖拽到了UIInventoryBar上
            if(eventData.pointerCurrentRaycast.gameObject!=null&& eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                int dstSlotID = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotID;
                if (slotID != dstSlotID)
                {
                    InventoryManager.Instance.SwapUIInventorySlot(InventoryLocation.Player,slotID, dstSlotID);
                }
                DestroyInventoryTextBox();
                ClearSelectedItem();
            }
            else
            {
                if (itemDetails.canDrop)
                {
                    DropSelectedItemAtMousePosition();
                }
            }
            PlayerController.Instance.EnablePlayerInput = true;
        }
    }

    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected == true)
        {
            if(gridCursor.CursorPositionIsValid)
            {
                //根据camera和鼠标位置，确定要放置的item的位置
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                    Input.mousePosition.y, -mainCamera.transform.position.z));
                //-Settings.gridCellSize/2f是因为item的基准点在最底部，而跟随鼠标的时候是中心跟随鼠标
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y-Settings.gridCellSize/2f, worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                //只需要给该item传入itemcode即可，对应的初始化在item.cs中
                item.ItemCode = itemDetails.itemCode;

                //从玩家背包中减少对应物品的数量
                InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
                if (InventoryManager.Instance.FindItemInventory(InventoryLocation.Player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }

            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform,false);
            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            //设置TextBox的文本
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);
            inventoryTextBox.SetTextBoxContent(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // 根据InventoryBar的位置设置TextBox的位置
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);

            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    private void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected == true){ClearSelectedItem();}
            else
            {
                if (itemQuantity > 0){SetSelectedItem();}
            }
        }

    }

    private void SetSelectedItem()
    {
        //重新设置高亮框
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = true;
        inventoryBar.SetHighlightInventorySlots();
        // 根据选中item的使用范围来决定是否启用gridCursor
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }
        gridCursor.SelectedItemType = itemDetails.itemType;
        // 把选中物体的itemCode保存在InventoryManager的一个数组中
        InventoryManager.Instance.SetSelectInventoryItem(InventoryLocation.Player, itemDetails.itemCode);
        // 根据该物体是否可被carry，调用对应方法
        if (itemDetails.canCarry == true)
        {
            PlayerController.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            PlayerController.Instance.ClearCarriedItem();
        }

    }

    private void ClearSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();
        ClearCursor();
        // 上侧函数与下侧两语句定位有重合
        isSelected = false;
        InventoryManager.Instance.ClearSelectInventoryItem(InventoryLocation.Player);
        PlayerController.Instance.ClearCarriedItem();
    }

    private void ClearCursor()
    {
        gridCursor.DisableCursor();
        gridCursor.SelectedItemType = ItemType.none;
    }

}
