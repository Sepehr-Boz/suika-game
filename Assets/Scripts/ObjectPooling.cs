using System.Collections.Generic;
using UnityEngine;

// GameManager singleton to hold common data that will be shared
public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance;
    public bool isReady;

    public GameObject[] itemsToPool;

    public int numInstances;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            Init();
        }
    }

    private void Init()
    {
        isReady = false;

        for (int j = 0; j < itemsToPool.Length; j++)
        {
            GameObject[] row = new GameObject[] { };
            for (int i = 0; i < numInstances; i++)
            {
                // instantiate obj and child to the empty parent
                GameObject newInstance = Instantiate(itemsToPool[j]);
                newInstance.transform.position = new Vector2(-100, -100);
                newInstance.transform.parent = this.transform;

                newInstance.GetComponent<ItemController>().lvl = j;

            }
        }


        isReady = true;
    }


    private GameObject FindInactiveObject(int lvl)
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf && child.gameObject.GetComponent<ItemController>().lvl == lvl) return child.gameObject;
        }

        // if out of inactive objects then instantiate new object and add to dictionary
        GameObject newInstance = Instantiate(itemsToPool[lvl]);
        newInstance.transform.position = new Vector2(-100, -100);
        newInstance.transform.parent = this.transform;
        newInstance.GetComponent<ItemController>().lvl = lvl;

        return newInstance;
    }

    public GameObject GetObject(int lvl, Vector3 positionToSpawn)
    {
        GameObject obj = FindInactiveObject(lvl);
        obj.transform.position = positionToSpawn;
        obj.SetActive(true);
        return obj;
    }

    public void StoreObject(GameObject obj)
    {
        obj.SetActive(false);
        // move the objects out of the way
        obj.transform.position = new Vector2(-100, -100);
    }


    public GameObject[] GetObjsActive()
    {
        List<GameObject> activeObjs = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) activeObjs.Add(child.gameObject);
        }

        return activeObjs.ToArray();
    }
}
