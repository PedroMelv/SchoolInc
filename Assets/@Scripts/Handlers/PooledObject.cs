using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public static List<ObjectPool> All_Pools = new List<ObjectPool>();

    public static string POOLFOLDERPATH = "PooledObjects/";
    public static int POOL_SIZE = 15;

    private GameObject poolObjectReference;

    private string objectName;
    public string ObjectName => objectName;
    private string fullPath => POOLFOLDERPATH + objectName;

    private PooledObject[] pool;

    public static ObjectPool CreatePool(GameObject objectRef)
    {
        for (int i = 0; i < All_Pools.Count; i++)
        {
            if (All_Pools[i].poolObjectReference == objectRef)
            {
                return All_Pools[i];
            }
        }

        ObjectPool pooledObject = new ObjectPool();

        return pooledObject.CreatePool_Internal(objectRef);
    }

    private ObjectPool CreatePool_Internal(GameObject objectRef)
    {
        for (int i = 0; i < All_Pools.Count; i++)
        {
            if (All_Pools[i].poolObjectReference == objectRef)
            {
                return All_Pools[i];
            }
        }

        poolObjectReference = objectRef;

        InitializePool();

        return this;
    }

    public static ObjectPool CreatePool(string objectName)
    {
        for (int i = 0; i < All_Pools.Count; i++)
        {
            if (All_Pools[i].ObjectName == objectName)
            {
                return All_Pools[i];
            }
        }

        ObjectPool pooledObject = new ObjectPool();

        return pooledObject.CreatePool_Internal(objectName);
    }
    private ObjectPool CreatePool_Internal(string objectName)
    {
        for (int i = 0; i < All_Pools.Count; i++)
        {
            if (All_Pools[i].ObjectName == objectName)
            {
                return All_Pools[i];
            }
        }

        this.objectName = objectName;

        poolObjectReference = Resources.Load<GameObject>(fullPath);

        InitializePool();

        return this;
    }

    private void InitializePool()
    {
        pool = new PooledObject[POOL_SIZE];

        for (int i = 0; i < POOL_SIZE; i++)
        {
            pool[i] = CreatePoolObject();
        }

        All_Pools.Add(this);
    }
    private void IncreasePoolSize()
    {
        PooledObject[] newPool = new PooledObject[pool.Length + POOL_SIZE];

        for (int i = 0; i < pool.Length; i++)
        {
            newPool[i] = pool[i];
        }

        for (int i = pool.Length; i < newPool.Length; i++)
        {
            newPool[i] = CreatePoolObject();
        }

        pool = newPool;
    }

    private PooledObject CreatePoolObject()
    {
        GameObject obj = GameObject.Instantiate(poolObjectReference);
        obj.SetActive(false);
        //obj.hideFlags = HideFlags.HideInHierarchy;

        PooledObject pooledObject = new PooledObject(obj);

        return pooledObject;
    }

    public bool TryGetPooledObject(GameObject obj, out PooledObject pooled)
    {
        pooled = GetPooledObject(obj);

        return pooled != null;
    }

    public PooledObject GetPooledObject(GameObject obj)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].gameObject == obj)
            {
                return pool[i];
            }
        }

        return null;
    }
    public bool IsOnPool(GameObject obj)
    {
        return GetPooledObject(obj) != null;
    }

    public PooledObject Instantiate(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].isUsed)
            {
                pool[i].gameObject.SetActive(true);
                pool[i].gameObject.transform.position = position;
                pool[i].gameObject.transform.rotation = rotation;
                pool[i].isUsed = true;
                return pool[i];
            }
        }

        IncreasePoolSize();

        return Instantiate(position,rotation);
    }
}
public class PooledObject
{
    public GameObject gameObject;
    public bool isUsed = false;

    public PooledObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = Vector3.zero;
        isUsed = false;
    }
}
