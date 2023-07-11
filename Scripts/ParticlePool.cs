using UnityEngine;
using System.Collections.Generic;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool instance;
    public GameObject prefab;
    public int poolSize = 20;

    private List<GameObject> pool;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.AddComponent<PoolObject>().IsPooled = true; // add the PoolObject component and set the flag
            pool.Add(obj);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab);
        newObj.AddComponent<PoolObject>().IsPooled = true; // add the PoolObject component and set the flag
        pool.Add(newObj);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        PoolObject poolObject = obj.GetComponent<PoolObject>(); // get the PoolObject component
        if (poolObject != null && poolObject.IsPooled) // check if it's a pooled object
        {
            obj.SetActive(false);
        }
        else
        {
            Destroy(obj);
        }
    }
}

public class PoolObject : MonoBehaviour
{
    public bool IsPooled { get; set; }
}
