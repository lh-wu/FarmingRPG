using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom
    {
        set { isInventoryBarPositionBottom = value; }
        get { return isInventoryBarPositionBottom; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }


    /// <summary>
    /// ����player����������λ�ã�������Ʒ���ڽ��涥�����ǵײ�
    /// </summary>
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewPointPosition = PlayerController.Instance.GetPlayerPosition();
        if(playerViewPointPosition.y>0.3&& IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);
            IsInventoryBarPositionBottom = true;
        }
        else if(playerViewPointPosition.y <= 0.3 && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);
            IsInventoryBarPositionBottom = false;
        }
    }
}
