using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehavior<SceneItemsManager>, ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _ISavealeUniqueID;
    public string ISaveableUniqueID { get { return _ISavealeUniqueID; } set { _ISavealeUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    /// <summary>
    /// ��ȡ�³�����itemparent��transformλ��
    /// </summary>
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }
    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    /// <summary>
    /// ��SaveLoadManager���Ƴ�
    /// </summary>
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    /// <summary>
    /// ��ӵ�SaveLoadManager��
    /// </summary>
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    /// <summary>
    /// ��sceneName�ĳ����е�������Ϣ���浽�õ���ģʽ��manager�У�
    /// ����յ���ģʽmanager�е���Ϣ�������
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        //itemsInSceneΪ��ǰ�����ڵ�item�б�
        //����Ҫ����ת��ΪSceneItem��ʽ��list����sceneItemList
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();
        foreach(Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);
        }


        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;
        //�ѵ�ǰ����Ҫ�����������ӵ�GameObjectSave.sceneData��
        GameObjectSave.sceneData.Add(sceneName, sceneSave);

    }

    /// <summary>
    /// ���ݵ���ģʽmanager�б������Ϣ���ɳ����еĿɱ������壻
    /// ����յ�ǰ���������壬�����ɡ�
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName,out SceneSave sceneSave))
        {
            if(sceneSave.listSceneItem!=null)
            {
                DestorySceneItems();
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }

    /// <summary>
    /// �ѵ�ǰ�����еĿɱ����objectɾ��
    /// </summary>
    private void DestorySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();
        for (int i = itemsInScene.Length - 1; i >= 0; --i)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }


    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;
        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(itemPosition.x, itemPosition.y, itemPosition.z), Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }
}
