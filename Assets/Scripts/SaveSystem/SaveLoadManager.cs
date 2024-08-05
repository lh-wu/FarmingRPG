using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    // iSaveableObjectList�е�ÿһ��Ԫ�ض�Ӧһ�ֱ������������ͼ������Ϊһ�֣���ͼ����״̬Ϊ��һ��
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
        // һ�����л��뷴���л��Ĺ���
        BinaryFormatter bf = new BinaryFormatter();
        // �Ȳ鿴�Ƿ��пɶ�ȡ���ļ�
        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            // ��ȡ�������ļ�����������GameSave����
            gameSave = new GameSave();
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(file);
            // ����ÿһ�����������鿴�ļ����Ƿ��ж�Ӧ�Ķ��������򸲸�mem�е�����
            for(int i = iSaveableObjectList.Count - 1; i >= 0; --i)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    // Ϊ��ҪDestory gameObject����Ҫ��ת��ΪComponet����
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
        // ��ȡÿ�������������ݣ���ӵ�gameSave��
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }
        // ����һ���ļ�
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);
        // ��gameSave���浽�ļ��У������Ƹ�ʽ��
        bf.Serialize(file, gameSave);
        file.Close();
        UIManager.Instance.DisablePauseMenu();
    }
}
