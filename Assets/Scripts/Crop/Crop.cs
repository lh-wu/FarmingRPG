using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int cropGridPosition;

    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    private int harvestActionCount = 0;

    /// <summary>
    /// ��crop�����ո�������øõؿ����Ϣ����ӵ����ﵽ����
    /// </summary>
    public void ProcessToolAction(ItemDetails itemDetails, PickingDirection pickingDirection)
    {
        // �ɸ�crop��λ�û�ȡ����cropDetails
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null) { return; }
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null) { return ; }
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null) { return ;}

        Animator animator = GetComponentInChildren<Animator>();

        // ʹ�ù��߶Ը�crop�����ջ�ʱ�Ķ�������ľҡҷ�ȣ�
        bool isToolRight, isToolLeft, isToolDown, isToolUp;
        isToolRight = isToolLeft = isToolDown = isToolUp = false;
        if (pickingDirection == PickingDirection.Left) { isToolLeft = true; }
        else if(pickingDirection == PickingDirection.Right) { isToolRight = true; }
        else if (pickingDirection == PickingDirection.Up) { isToolUp = true; }
        else { isToolDown = true; }
        if (animator != null)
        {
            if (isToolRight || isToolUp) { animator.SetTrigger("usetoolright"); }
            else if (isToolLeft || isToolDown) { animator.SetTrigger("usetoolleft"); }
        }

        // �����Ҫ�ո�Ĵ����Լ��Ƿ�ʹ�øù����ո�
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
        // �����ջ񶯻���sprite
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            if (cropDetails.harvestedSprite != null&& cropHarvestedSpriteRenderer != null)
            {
                cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
            }
        // �����ջ񶯻�
        if (isUsingToolRight || isUsingToolUp) { animator.SetTrigger("harvestright"); }
        else { animator.SetTrigger("harvestleft"); }
        }
        // �������õؿ���Ϣ
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;
        // �����ջ񶯻�ʱ����ԭ�������ͼ
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // ������䣨����gridProperty�������ǲ���Ҫ��
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        // �ȴ�����������Ϻ�����ӵ���Ʒ����ɾ��object
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
        SpawnHarvestedItems(cropDetails);
        Destroy(gameObject);
    }

    // ����cropDetails���ɵ�����÷�����Ҫ����������������λ�ã�
    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for(int i=0;i<cropDetails.cropProducedItemCode.Length;i++)
        {
            // ����������������
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
            // ����������λ��
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
