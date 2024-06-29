[System.Serializable]
public struct CharacterAttribute
{
    // �ýṹ������˵��������Ҫ�����Ķ����ı���
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