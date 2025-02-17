using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int cropGridPosition;

    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    [SerializeField] private Transform harvestActionEffectTransform = null;

    private int harvestActionCount = 0;

    /// <summary>
    /// 对crop进行收割，重新设置该地块的信息，添加掉落物到背包
    /// </summary>
    public void ProcessToolAction(ItemDetails itemDetails, bool isToolRight, bool isToolLeft, bool isToolUp, bool isToolDown)
    {
        // 由该crop的位置获取到该cropDetails
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null) { return; }
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null) { return ; }
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null) { return ;}

        Animator animator = GetComponentInChildren<Animator>();

        // 使用工具对该crop进行收获时的动画（树木摇曳等）
        if (animator != null)
        {
            if (isToolRight || isToolUp) { animator.SetTrigger("usetoolright"); }
            else if (isToolLeft || isToolDown) { animator.SetTrigger("usetoolleft"); }
        }
        // 触发粒子效果
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }
        // 检查需要收割的次数以及是否使用该工具收割
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(itemDetails.itemCode);
        if(requiredHarvestActions == -1) { return; }
        harvestActionCount++;
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight,isToolUp,cropDetails, gridPropertyDetails,animator);
        }
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp,CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        // 设置收获动画的sprite
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            if (cropDetails.harvestedSprite != null&& cropHarvestedSpriteRenderer != null)
            {
                cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
            }
        // 触发收获动画
        if (isUsingToolRight || isUsingToolUp) { animator.SetTrigger("harvestright"); }
        else { animator.SetTrigger("harvestleft"); }
        }
        // 重新设置地块信息
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;
        // 播放收获动画时隐藏原作物的贴图
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // 部分收获sprite含有boxcollider2d，在播放其上升动画的时候依然会触发碰撞，此处禁用这些collider
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        // 下面语句（更新gridProperty）可能是不必要的
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        // 等待动画播放完毕后再添加到物品栏并删除object
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }
        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // 生成掉落物
        SpawnHarvestedItems(cropDetails);
        // 如果该作物被收获后需要被转换为另一种作物
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }
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

    /// <summary>
    /// 将对应grid的种植信息设置为cropDetails.harvestedTransformItemCode，并显示crop
    /// </summary>
    /// <param name="cropDetails"></param>
    /// <param name="gridPropertyDetails"></param>
    private void CreateHarvestedTransformCrop(CropDetails cropDetails,GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}
