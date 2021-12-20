using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
