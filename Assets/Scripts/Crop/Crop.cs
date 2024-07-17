using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int cropGridPosition;

    private int harvestActionCount = 0;


    public void ProcessToolAction(ItemDetails itemDetails)
    {
        // 由该crop的位置获取到该cropDetails
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null) { return; }
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null) { return ; }
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null) { return ;}
        // 检查需要收割的次数以及是否使用该工具收割
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(itemDetails.itemCode);
        if(requiredHarvestActions == -1)
        {
            return;
        }
        harvestActionCount++;
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails);
        }
    }

    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;
        // 下面语句可能是不必要的
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);
        Destroy(gameObject);
    }

    // 根据cropDetails生成掉落物（该方法主要计算掉落物的数量和位置）
    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for(int i=0;i<cropDetails.cropProducedItemCode.Length;i++)
        {
            // 计算各掉落物的数量
            int cropsToProduce;
            if (cropDetails.cropProducedMaxQuantity[i]== cropDetails.cropProducedMinQuantity[i]||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }
            // 计算掉落物的位置
            for(int j=0;j<cropsToProduce;j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.Player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x+Random.Range(-1f,1f), transform.position.y + Random.Range(-1f, 1f),0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i],spawnPosition);
                }
            }
        }
    }


}
