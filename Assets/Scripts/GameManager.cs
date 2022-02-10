using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance = null;
    public static GameManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    public delegate void JoinGame();
    public JoinGame joinGame;
    public delegate void GameStart();
    public GameStart gameStart;

    public delegate void GameOver();
    public GameOver gameOver;

    private Transform spawnPoint1 = null;
    private Transform spawnPoint2 = null;
    
    public bool isPlaying = true;

    private void Config()
    {
        joinGame += m_joinGame;
        gameStart += m_GameStart;
        gameOver += m_GameOver;


        // Set SpawnPoint
        spawnPoint1 = GameObject.Find("SpawnPoint1").transform;
        spawnPoint2 = GameObject.Find("SpawnPoint2").transform;
        SpawnPlayer();
    }
    private void Awake() 
    {
        if(m_instance == null) m_instance = this;
        else Destroy(this.gameObject);

        Config();
    }

    private void Start()
    {
        gameStart();
    }

    private void m_joinGame()
    {
        StartCoroutine(WaitForStart());
    }
    private void m_GameStart()
    {
        isPlaying = true;
    }
    private void m_GameOver()
    {
        isPlaying = false;
        Debug.Log("GameManager : GameOver");
    }

    private void SpawnPlayer()
    {
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        player.transform.position = spawnPoint1.position;
        player.transform.rotation = spawnPoint1.rotation;
    }

    private IEnumerator WaitForStart()
    {
        yield return null;
        
    }
}
