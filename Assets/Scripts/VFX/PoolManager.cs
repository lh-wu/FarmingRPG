using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehavior<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private Pool[] pool = null;                    // ����˵����pool��prefab��size
    [SerializeField] private Transform objectPoolTransform = null;

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        for(int i = 0; i < pool.Length; ++i)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        // objectPoolTransform�����г��ӵĸ����󣬶�parentGameObject(����)��ĳһ����prefab�ĸ�����
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for(int i = 0; i < poolSize; ++i)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab,Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);
            ResetObject(position, rotation, objectToReuse, prefab);
            return objectToReuse;
        }
        else
        {
            Debug.Log("No suitable pool for this instance:" + prefab);
            return null;
        }
    }


    private GameObject GetObjectFromPool(int poolKey)
    {
        // ���������������µ���pool�ڵ�˳�򣨸ձ�ȡ����object�ŵ��˶�β��
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(objectToReuse);
        // �������ȡ������active״̬�ģ���Ҫ����Ϊ��activate
        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }
        return objectToReuse;
    }

    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse,GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }

}
