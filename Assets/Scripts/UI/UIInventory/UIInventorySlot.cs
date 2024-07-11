using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Camera mainCamera;
    private GameObject draggedItem;
    private Transform parentItem;           //����Ʒ���ϵ�������Ӧ�ý�����ൽparentItem����Ŀ¼����(Hierachy����)
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
    /// �ڳ����л���ʱ�����parentItemΪ��ǰ�����µ�item��һ����Ϸobject���ó���������item�����������У�
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
            // �˴�ѡ��inventoryBar.transform��������ΪinventoryBar��canvas���������ʹ����ק����������ʾ
            // ��Instantiate��������ʼ����object����inventoryBar��������
            // �ô���ʼ����λ�����ϻᱻOnEndDrag����Ϊ������꣬���Գ�ʼλ�ò���Ҫ
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            SetSelectedItem();
        }
    }

    /// <summary>
    /// ��ק������������λ��
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

            // ��if�ж������Ƿ���ק����UIInventoryBar��
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
                //����camera�����λ�ã�ȷ��Ҫ���õ�item��λ��
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                    Input.mousePosition.y, -mainCamera.transform.position.z));
                //-Settings.gridCellSize/2f����Ϊitem�Ļ�׼������ײ�������������ʱ�������ĸ������
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y-Settings.gridCellSize/2f, worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                //ֻ��Ҫ����item����itemcode���ɣ���Ӧ�ĳ�ʼ����item.cs��
                item.ItemCode = itemDetails.itemCode;

                //����ұ����м��ٶ�Ӧ��Ʒ������
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

            //����TextBox���ı�
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);
            inventoryTextBox.SetTextBoxContent(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // ����InventoryBar��λ������TextBox��λ��
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
        //�������ø�����
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = true;
        inventoryBar.SetHighlightInventorySlots();
        // ����ѡ��item��ʹ�÷�Χ�������Ƿ�����gridCursor
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
        // ��ѡ�������itemCode������InventoryManager��һ��������
        InventoryManager.Instance.SetSelectInventoryItem(InventoryLocation.Player, itemDetails.itemCode);
        // ���ݸ������Ƿ�ɱ�carry�����ö�Ӧ����
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
        // �ϲຯ�����²�����䶨λ���غ�
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
