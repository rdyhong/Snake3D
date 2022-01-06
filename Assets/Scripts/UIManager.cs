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
    public Text coinText;
    public int coin {get; private set;}
    public Text scoreText;
    public int score {get; private set;}

    private void Awake() 
    {
        coin = 0;
        score = 0;
        scoreText.text = score.ToString();
        coinText.text = coin.ToString();
        gameOver_T.enabled = false;
    }
    void Start()
    {
        
    }

    public void GameOver()
    {
        gameOver_T.enabled = true;
    }

    public void AddScoreAndCoin(int sc, int co)
    {
        score += sc;
        coin += co;
        scoreText.text = score.ToString();
        coinText.text = coin.ToString();
    }


}
