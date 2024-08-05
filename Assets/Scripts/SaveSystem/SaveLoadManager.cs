using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    // iSaveableObjectList中的每一个元素对应一种保存器，比如地图掉落物为一种，地图格子状态为另一种
    public List<ISaveable> iSaveableObjectList;

    public GameSave gameSave;


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

    public void LoadDataFromFile()
    {
        // 一个序列化与反序列化的工具
        BinaryFormatter bf = new BinaryFormatter();
        // 先查看是否有可读取的文件
        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            // 读取二进制文件，并解析成GameSave类型
            gameSave = new GameSave();
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(file);
            // 对于每一个保存器，查看文件中是否有对应的东西，有则覆盖mem中的内容
            for(int i = iSaveableObjectList.Count - 1; i >= 0; --i)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    // 为了要Destory gameObject，需要先转换为Componet类型
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            file.Close();
        }
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();
        // 获取每个保存器的内容，添加到gameSave中
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }
        // 创建一个文件
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);
        // 将gameSave保存到文件中（二进制格式）
        bf.Serialize(file, gameSave);
        file.Close();
        UIManager.Instance.DisablePauseMenu();
    }
}
