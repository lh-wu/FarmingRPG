using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;
    private SpriteRenderer spriteRenderer;

    public int ItemCode
    {
        get { return _itemCode; }
        set { _itemCode = value; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode!=0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodeParam)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            spriteRenderer.sprite = itemDetails.itemSprite;

            //如果该item是ItemType.ReapableScenary类型，则为其添加 碰撞-摇晃 的c#脚本
            if (itemDetails.itemType == ItemType.ReapableScenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }

        }
    }
}
