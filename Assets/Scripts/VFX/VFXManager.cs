using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehavior<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPrefab = null;

    protected override void Awake()
    {
        base.Awake();
        twoSeconds = new WaitForSeconds(1);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= displayHarvestActionEffect;
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += displayHarvestActionEffect;
    }

    /// <summary>
    /// 等待secondsToWait秒后，将effectGameObject的active状态设置为false
    /// </summary>
    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void displayHarvestActionEffect(Vector3 effectPosition,HarvestActionEffect harvestActionEffect)
    {
        switch(harvestActionEffect)
        {
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;
            case HarvestActionEffect.none:
                break;
                default: break;
        }
    }
}
