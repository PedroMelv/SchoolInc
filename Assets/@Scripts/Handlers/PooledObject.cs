using System.Collections.Generic;
using UnityEngine;

public class PooledObject
{
    //public static List<PooledObject> allPools = new List<PooledObject>();

    public const int DEFAULT_POOL_SIZE = 10;

    private List<GameObject> unavailable = new List<GameObject>();
    private Queue<GameObject> available = new Queue<GameObject>();

    private GameObject prefab;

    public static PooledObject CreateOrFind(GameObject prefab)
    {
        //if(allPools.Count > 0)
        //{
        //    for (int i = 0; i < allPools.Count; i++)
        //    {
        //        if (allPools[i].prefab == prefab) return allPools[i];
        //    }
        //}
        return new PooledObject(prefab);
    }

    public PooledObject(GameObject prefab)
    {
        this.prefab = prefab;

        IncreasePool();
        //allPools.Add(this);
    }

    private void IncreasePool()
    {
        for (int i = 0; i < DEFAULT_POOL_SIZE; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, new Vector3(1000,-1000,1000), Quaternion.identity);
            obj.gameObject.SetActive(false);
            available.Enqueue(obj);
        }
    }

    public GameObject Instantiate(Vector3 position, Quaternion rotation)
    {
        if (available.Count == 0)
        {
            IncreasePool();
        }

        GameObject pooledObject = available.Dequeue();
        unavailable.Add(pooledObject);
        
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        pooledObject.gameObject.SetActive(true);


        return pooledObject;
    }

    public void Return(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        unavailable.Remove(obj);
        available.Enqueue(obj);
    }
}
