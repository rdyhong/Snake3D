using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    CameraMovement cameraMovement;
    public static GameManager m_instance;
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

    public GameObject tailPf;
    public GameObject coinPf;

    void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
        SpawnMeal();
        StartCoroutine(CoinCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    
    public void SpawnMeal()
    {
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);
        Vector3 randPos = new Vector3(randX, 0, randY);
        TailsController tail = Instantiate(tailPf, randPos, Quaternion.identity).GetComponent<TailsController>();
    }

    public void SpawnCoin()
    {
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);
        Vector3 randPos = new Vector3(randX, 0, randY);
        Instantiate(coinPf, randPos, Quaternion.Euler(new Vector3(-90, 0, 0)));
    }
    IEnumerator CoinCoroutine()
    {
        while(true)
        {
            SpawnCoin();
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void GameOver()
    {
        Debug.Log("GameManager : GameOver");
        //cameraMovement.IsPlayerDead(true);
    }
}
