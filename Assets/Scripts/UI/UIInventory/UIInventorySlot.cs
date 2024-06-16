using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private Camera mainCamera;
    private Transform parentItem;           //当物品被拖到场景后，应该将其归类到parentItem的子目录里面(Hierachy界面)
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;


    private void Start()
    {
        mainCamera = Camera.main;
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
                // TODO 进行换位操作
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
        if (itemDetails != null)
        {
            //根据camera和鼠标位置，确定要放置的item的位置
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            //只需要给该item传入itemcode即可，对应的初始化在item.cs中
            item.ItemCode = itemDetails.itemCode;

            //从玩家背包中减少对应物品的数量
            InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
        }
    }
}
