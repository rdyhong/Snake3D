using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private static ObstacleManager m_instance;
    public static ObstacleManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<ObstacleManager>();
            }
            return m_instance;
        }
    }

    private GameObject[] obstacles;
    private List<GameObject> allObstacle = new List<GameObject>();
    private GameObject cautionObj = null;

    private void InitSet()
    {
        obstacles = Resources.LoadAll<GameObject>("Obstacles");
        cautionObj = Resources.Load<GameObject>("Caution");
    }

    private void Awake()
    {
        InitSet();
        GameManager.instance.gameStart += GameStart;
        GameManager.instance.gameOver += GameOver;
    }

    private void GameStart()
    {
        Spawn(true);
    }
    private void GameOver()
    {
        Spawn(false);

        // Remove all Obstacle
        while(allObstacle.Count > 0)
        {
            GameObject obj = allObstacle[0];
            allObstacle.Remove(obj);
            Destroy(obj);
        }
    }

    private void SpawnRandomObstacle()
    {
        float randX = Random.Range(-10f, 10f);
        float randZ = Random.Range(-10f, 10f);
        Vector3 randPos = new Vector3(randX, 10, randZ);

        // Caution Prefab set Position, spawn
        Vector3 cautionPos = new Vector3(randX, -0.8f, randZ);
        Instantiate(cautionObj, cautionPos, Quaternion.identity);

        float rotX = Random.Range(0f, 360f);
        float rotY = Random.Range(0f, 360f);
        float rotZ = Random.Range(0f, 360f);
        Quaternion rot = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));

        int randomObstacleIdx = Random.Range(0, obstacles.Length);
        GameObject obj = Instantiate(obstacles[randomObstacleIdx], randPos, rot);
        allObstacle.Add(obj);
    }
    
    private IEnumerator co = null;
    private void Spawn(bool isStart)
    {
        if(isStart)
        {
            co = SpawnCo();
            StartCoroutine(co);
        }
        else
        {
            if(co != null) StopCoroutine(co);
        }
    }
    private IEnumerator SpawnCo()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            SpawnRandomObstacle();
        }
    }
}
