using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisPool : MonoBehaviour
{
    public static DebrisPool m_instance;
    public static DebrisPool instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<DebrisPool>();
            }
            return m_instance;
        }
    }

    private GameObject poolingObjPrefab;
    private Queue<GameObject> poolingObjQueue = new Queue<GameObject>();

    private void Config()
    {
        poolingObjPrefab = Resources.Load<GameObject>("Debris");
        Initialize(300);
    }
    private void Awake()
    {
        if(m_instance == null)
        {
            m_instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Config();
    }

    private GameObject CreateNewGameObject()
    {
        var newObj = Instantiate(poolingObjPrefab, transform.position, Quaternion.identity, this.transform);
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
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        instance.poolingObjQueue.Enqueue(obj);
    }
}
