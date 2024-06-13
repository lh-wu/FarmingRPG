
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //�������Ե���inspector������ֵĸ߶ȱ�Ϊԭ��������
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if(property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();       //����ֵ�ı仯

            //��ʾitemCode
            // labelΪinspector����ƫ��ı�����
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            //��ʾitemDescription
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));


            //��������˱仯�����Ϊ�µ�ֵ
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();

    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_ItemList;
        so_ItemList = AssetDatabase.LoadAssetAtPath("Assets/SO Assets/Item/so_ItemList.asset",typeof(SO_ItemList)) as SO_ItemList;
        List<ItemDetails> itemDetailsList = so_ItemList.itemDetails;

        ItemDetails targetItemDetails = itemDetailsList.Find(x => x.itemCode == itemCode);
        if (targetItemDetails == null)
        {
            return "";
        }
        return targetItemDetails.itemDescription;
        

    }
}
