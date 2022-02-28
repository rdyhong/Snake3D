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

    private GameObject canvas = null;

    public PlayerController pc;
    private Text gameOverText;
    public Text coinText;
    public Text scoreText;
    public int coin { get; private set; }
    public int score { get; private set; }


    private void Init()
    {
        canvas = GameObject.Find("Canvas");
        gameOverText = canvas.transform.Find("GameOverText").GetComponent<Text>();;
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
        
        Init();

        GameManager.instance.joinGame += JoinGame;
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
        Vector3 curPos = gameOverText.transform.position;
        Vector3 targetPos = gameOverText.transform.position + new Vector3(0, -50f, 0);
        gameOverText.transform.DOMove(targetPos, 1.5f);

        yield return new WaitUntil(()=>gameOverText.transform.position == targetPos);

        yield return new WaitForSeconds(5f);
        gameOverText.transform.DOMove(curPos, 1.5f);
    }

    private void JoinGame()
    {
        StartCoroutine(OnLoadScene());
    }
    private IEnumerator OnLoadScene()
    {
        GameObject coverImgObj = canvas.transform.Find("CoverImage").gameObject;
        Image coverImg = coverImgObj.GetComponent<Image>();

        coverImgObj.SetActive(true);
        coverImg.DOColor(new Color(coverImg.color.r, coverImg.color.g, coverImg.color.b, 0), 1f);

        yield return new WaitUntil(() => coverImg.color.a == 0);

        coverImgObj.SetActive(false);
    }
}
