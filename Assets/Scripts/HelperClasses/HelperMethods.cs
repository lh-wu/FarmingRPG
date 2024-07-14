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

    /// <summary>
    /// Ѱ��ĳһ��λ�õģ��������T��object�������List<T>����
    /// </summary>
    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        T tcomponent = default(T);
        for(int i = 0; i < collider2DArray.Length; ++i)
        {
            tcomponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tcomponent != null)
            {
                found = true;
                componentList.Add(tcomponent);
            }
            else
            {
                tcomponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tcomponent != null)
                {
                    found = true;
                    componentList.Add(tcomponent);
                }
            }
        }
        componentsAtPositionList = componentList;
        return found;
    }

    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersTotest,Vector2 point,Vector2 size,float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersTotest];
        // OverlapBoxNonAllocͬ��ΪѰ��һ�����η�Χ��collider����
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);
        T tcomponent = default(T);
        T[] tcomponentArray = new T[numberOfCollidersTotest];
        for(int i = collider2DArray.Length - 1; i >= 0; --i)
        {
            if (collider2DArray[i] != null)
            {
                tcomponent = collider2DArray[i].gameObject.GetComponent<T>();
                if (tcomponent != null)
                {
                    tcomponentArray[i] = tcomponent;
                }
            }
        }
        return tcomponentArray;
    }


}
