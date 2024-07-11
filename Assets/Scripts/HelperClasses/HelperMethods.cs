using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{

    /// <summary>
    /// ����2d�����Ĵ�����λ�á���С���Ƕȣ�����collider����Ƿ����T���͵�GameObject��������
    /// </summary>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition,Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T> ();
        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size,angle);
        for(int i = 0; i < collider2DArray.Length; ++i)
        {
            T tcomponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tcomponent != null)
            {
                found = true;
                componentList.Add(tcomponent);
            }
            else
            {
                tcomponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if(tcomponent != null)
                {
                    found = true;
                    componentList.Add(tcomponent);
                }
            }
        }
        listComponentsAtBoxPosition = componentList;
        return found;
    }
}
