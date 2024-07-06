using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    // ISaveable为单例模式，推测为运行时保存在内存的manager或是保存到本地文件的manager
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();
        iSaveableObjectList = new List<ISaveable>();
    }
    public void StoreCurrentSceneData()
    {
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

}
