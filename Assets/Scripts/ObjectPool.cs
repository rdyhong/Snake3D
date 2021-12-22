using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool m_instance;
    public static ObjectPool instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<ObjectPool>();
            }
            return m_instance;
        }
    }

    [SerializeField]
    private GameObject poolingObjPrefab;
    private Queue<GameObject> poolingObjQueue = new Queue<GameObject>();

    private void Awake()
    {
        Initialize(100);
    }

    private GameObject CreateNewGameObject()
    {
        var newObj = Instantiate(poolingObjPrefab, transform);
        newObj.SetActive(false);
        return newObj;
    }

    private void Initialize(int count)
    {
        for(int i = 0; i < count; i++)
        {
            poolingObjQueue.Enqueue(CreateNewGameObject());
        }
    }

    public static GameObject GetDebris()
    {
        if(instance.poolingObjQueue.Count > 0)
        {
            var obj = instance.poolingObjQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true);
            return obj;

        }
        else
        {
            var newObj = instance.CreateNewGameObject();
            newObj.transform.SetParent(null);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnDebris(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.poolingObjQueue.Enqueue(obj);
    }
}
