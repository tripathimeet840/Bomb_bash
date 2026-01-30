using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class PoolItem
    {
        public string prefabName;
        public GameObject prefab;
        public int poolSize = 10;
    }

    public List<PoolItem> poolItems;
    public static ObjectPooler instance;
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        foreach (PoolItem item in poolItems)
        {
            prefabDictionary.Add(item.prefabName, item.prefab);

            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(item.prefabName, objectPool);
        }
    }

    public GameObject GetObject(string prefabName)
    {
        if (poolDictionary.ContainsKey(prefabName))
        {
            foreach (GameObject obj in poolDictionary[prefabName])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            if (prefabDictionary.ContainsKey(prefabName))
            {
                GameObject newObj = Instantiate(prefabDictionary[prefabName]);
                poolDictionary[prefabName].Add(newObj);
                return newObj;
            }
            else
            {
                Debug.LogWarning("Prefab name not found in object pool: " + prefabName);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Prefab name not found in object pool: " + prefabName);
            return null;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
