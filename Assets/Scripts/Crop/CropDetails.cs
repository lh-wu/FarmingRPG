using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;            // 种子的itemCode
    public int[] growthDays;            // 不同阶段的生长时间
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;      // 收获得到item的sprite

    [ItemCodeDescription]
    public int harvestedTransformItemCode;                      // 当一个crop被收获后可以转变为另一种crop（例如树木和树桩）
    public bool hideCropBeforeHarvestedAnimation;               // 在播放收获动画时是否隐藏原作物的sprite
    public bool disableCropCollidersBeforeHarvestedAnimation;

    public bool isHarvestedAnimation;                           // 是否有收获动画
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;              // 掉落物是否直接生成到玩家背包中
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;           // 可用于收获该crop的工具的itemCode，如果没有元素，则说明不需要特定工具
    public int[] requiredHarvestActions;        // 收获该crop需要的工具操作数量

    [ItemCodeDescription]
    public int[] cropProducedItemCode;          // 该crop被收获后，出产的item
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;


    /// <summary>
    /// 返回使用该工具收获crop需要的操作数
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
    /// 返回该工具是否可以收获该crop
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
