[System.Serializable]
public struct CharacterAttribute
{
    // 该结构体用于说明我们想要更换的动画的变体
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

    public CharacterAttribute(CharacterPartAnimator _characterPart, PartVariantColor _partVariantColor, PartVariantType _partVariantType)
    {
        characterPart = _characterPart;
        partVariantColor = _partVariantColor;
        partVariantType = _partVariantType;
    }
}