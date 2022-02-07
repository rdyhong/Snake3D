using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private GameObject coinPrefab;

    private void Awake()
    {
        coinPrefab = Resources.Load<GameObject>("Coin");

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
    }

    private void SpawnCoin()
    {
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);
        Vector3 randPos = new Vector3(randX, 0, randY);
        Instantiate(coinPrefab, randPos, Quaternion.Euler(new Vector3(-90, 0, 0)));
    }

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

    private IEnumerator co;
    private IEnumerator SpawnCo()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            SpawnCoin();
        }
    }
}
