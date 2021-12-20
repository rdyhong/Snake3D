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

    public GameObject tailPrefab;

    void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
        SpawnMeal();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            //Time.timeScale = 1;
        }
    }

    public void SpawnMeal()
    {
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);
        Vector3 randPos = new Vector3(randX, 0, randY);
        TailsController tail = Instantiate(tailPrefab, randPos, Quaternion.identity).GetComponent<TailsController>();
    }

    public void GameOver()
    {
        Debug.Log("GameManager : GameOver");
        //Time.timeScale = 0;
        //cameraMovement.IsPlayerDead(true);
    }
}
