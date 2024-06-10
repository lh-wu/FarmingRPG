
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;

    public short itemUseGridRadius;         //ʹ��grid˵����Χ
    public float itemUseRadius;             //�޷�ʹ��grid˵����Χ��radius

    public bool isStartingItem;
    public bool canPickUp;
    public bool canDrop;
    public bool canEat;
    public bool canCarry;
}
