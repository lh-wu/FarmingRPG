using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;            // ���ӵ�itemCode
    public int[] growthDays;            // ��ͬ�׶ε�����ʱ��
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;      // �ջ�õ�item��sprite

    [ItemCodeDescription]
    public int harvestedTransformItemCode;                      // ��һ��crop���ջ�����ת��Ϊ��һ��crop��������ľ����׮��
    public bool hideCropBeforeHarvestedAnimation;               // �ڲ����ջ񶯻�ʱ�Ƿ�����ԭ�����sprite
    public bool disableCropCollidersBeforeHarvestedAnimation;

    public bool isHarvestedAnimation;                           // �Ƿ����ջ񶯻�
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;              // �������Ƿ�ֱ�����ɵ���ұ�����
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;           // �������ջ��crop�Ĺ��ߵ�itemCode�����û��Ԫ�أ���˵������Ҫ�ض�����
    public int[] requiredHarvestActions;        // �ջ��crop��Ҫ�Ĺ��߲�������

    [ItemCodeDescription]
    public int[] cropProducedItemCode;          // ��crop���ջ�󣬳�����item
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;


    /// <summary>
    /// ����ʹ�øù����ջ�crop��Ҫ�Ĳ�����
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for(int i = 0;i<harvestToolItemCode.Length;++i)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }


    /// <summary>
    /// ���ظù����Ƿ�����ջ��crop
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
