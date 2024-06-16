using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private Camera mainCamera;
    private Transform parentItem;           //����Ʒ���ϵ�������Ӧ�ý�����ൽparentItem����Ŀ¼����(Hierachy����)
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
            // �˴�ѡ��inventoryBar.transform��������ΪinventoryBar��canvas���������ʹ����ק����������ʾ
            // ��Instantiate��������ʼ����object����inventoryBar��������
            // �ô���ʼ����λ�����ϻᱻOnEndDrag����Ϊ������꣬���Գ�ʼλ�ò���Ҫ
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
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
                // TODO ���л�λ����
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
            //����camera�����λ�ã�ȷ��Ҫ���õ�item��λ��
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            //ֻ��Ҫ����item����itemcode���ɣ���Ӧ�ĳ�ʼ����item.cs��
            item.ItemCode = itemDetails.itemCode;

            //����ұ����м��ٶ�Ӧ��Ʒ������
            InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
        }
    }
}
