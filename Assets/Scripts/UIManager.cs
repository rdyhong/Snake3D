using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance = null;
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

    public PlayerController pc;
    public Text gameOver_T;
    public Text coinText;
    public Text scoreText;
    public int coin { get; private set; }
    public int score { get; private set; }


    private void Config()
    {
        coin = 0;
        score = 0;
        scoreText.text = score.ToString();
        coinText.text = coin.ToString();
        pc = FindObjectOfType<PlayerController>();
    }
    private void Awake()
    {
        if(m_instance == null) m_instance = this;
        else Destroy(this.gameObject);
        
        Config();
    }
    private void Start()
    {
        GameManager.instance.gameOver += GameOver;
    }

    public void AddScoreAndCoin(int sc = 0, int co = 0)
    {
        score += sc;
        coin += co;
        scoreText.text = score.ToString();
        coinText.text = coin.ToString();
    }

    public void GameOver()
    {
        StartCoroutine(MoveGameOverText());
    }

    private IEnumerator MoveGameOverText()
    {
        Vector3 curPos = gameOver_T.transform.position;
        Vector3 targetPos = gameOver_T.transform.position + new Vector3(0, -50f, 0);
        gameOver_T.transform.DOMove(targetPos, 1.5f);

        yield return new WaitUntil(()=>gameOver_T.transform.position == targetPos);

        yield return new WaitForSeconds(5f);
        gameOver_T.transform.DOMove(curPos, 1.5f);
    }
}
