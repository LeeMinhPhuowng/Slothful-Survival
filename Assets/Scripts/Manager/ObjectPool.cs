using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [System.Serializable]
    public class PoolInfo
    {
        public ObjectType type;
        public GameObject prefab;
        public int size;
    }

    public List<PoolInfo> poolInfos;
    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        InitializePools();
    }
    private void InitializePools()
    {
        poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();
        foreach (var poolInfo in poolInfos)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < poolInfo.size; i++)
            {
                GameObject obj = Instantiate(poolInfo.prefab, this.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(poolInfo.type, objectPool);
        }
    }

    public GameObject SpawnFromPool(ObjectType type, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            return null;
        }
        GameObject obj = poolDictionary[type].Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public GameObject SpawnFromPool(ObjectType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            return null;
        }
        GameObject obj = poolDictionary[type].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;  
        obj.SetActive(true);    
        return obj;
    }

    public GameObject SpawnFromPool(ObjectType type, Vector3 position, Quaternion rotation, System.Action<GameObject> onBeforeEnable = null)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"[ObjectPool] Pool with type {type} not found! Make sure it's added in the Inspector.");
            return null;
        }
        GameObject obj = poolDictionary[type].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        onBeforeEnable?.Invoke(obj);
        obj.SetActive(true);
        return obj;
    }

    public void BackToPool(GameObject obj, ObjectType type)
    {
        obj.SetActive(false);
        poolDictionary[type].Enqueue(obj);
    }
}
