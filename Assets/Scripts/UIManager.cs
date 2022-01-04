using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Text gameOver_T;

    private void Awake() 
    {
        gameOver_T.enabled = false;
    }
    void Start()
    {
        
    }

    public void GameOver()
    {
        gameOver_T.enabled = true;
    }
}
