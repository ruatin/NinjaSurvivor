using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;

    Queue<Bullet> poolObjectQueue = new Queue<Bullet>();

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private int poolAmount = 100;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            poolObjectQueue.Enqueue(CreateNewObject());
        }
    }

    private Bullet CreateNewObject()
    {
        var newObj = Instantiate(bulletPrefab).GetComponent<Bullet>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static Bullet GetObject()
    {
        if (instance.poolObjectQueue.Count > 0)
        {
            var obj = instance.poolObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnObject(Bullet obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.poolObjectQueue.Enqueue(obj);
    }
}
