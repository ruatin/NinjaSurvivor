using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPool : MonoBehaviour
{
    public static FloatingTextPool instance;

    Queue<DamageText> poolObjectQueue = new Queue<DamageText>();

    [SerializeField]
    private GameObject damageTextPrefab;
    [SerializeField]
    private int poolAmount = 100;
    
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    
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

    private DamageText CreateNewObject()
    {
        var newObj = Instantiate(damageTextPrefab,transform).GetComponent<DamageText>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static DamageText GetObject()
    {
        if (instance.poolObjectQueue.Count > 0)
        {
            var obj = instance.poolObjectQueue.Dequeue();
            return obj;
        }
        else
        {
            var newObj = instance.CreateNewObject();
//            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnObject(DamageText obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.poolObjectQueue.Enqueue(obj);
    }
}
