
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;

    public short itemUseGridRadius;         //使用grid说明范围
    public float itemUseRadius;             //无法使用grid说明范围的radius

    public bool isStartingItem;
    public bool canPickUp;
    public bool canDrop;
    public bool canEat;
    public bool canCarry;
}
