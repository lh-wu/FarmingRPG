using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIInventorySlot : MonoBehaviour
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

}
