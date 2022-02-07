using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySpawner : MonoBehaviour
{
    GameObject bodyPrefab;
    private List<GameObject> bodys = new List<GameObject>();
    
    private void Awake() 
    {
        bodyPrefab = Resources.Load<GameObject>("Body");
    }

    private void Start()
    {
        StartSpawning();
    }
    
    private void SpawnBody()
    {
        bool ready = false;
        while(!ready)
        {
            float randX = Random.Range(-10f, 10f);
            float randY = Random.Range(-10f, 10f);
            Vector3 randPos = new Vector3(randX, 0, randY);

            if(CheckSpawnPoint(randPos))
            {
                ready = true;
            }
            else
            {  
                continue;
            }
            GameObject tail = Instantiate(bodyPrefab, randPos, Quaternion.identity);
            bodys.Add(tail);
        }
    }

    private bool CheckSpawnPoint(Vector3 pos)
    {
        bool spawnAble = false;
        PlayerController[] pcs = FindObjectsOfType<PlayerController>();
        List<GameObject> players = new List<GameObject>();
        for(int i = 0; i < pcs.Length; i++)
        {
            players.Add(pcs[i].gameObject);
        }
        for(int k = 0; k < players.Count; k++)
        {
            if((players[k].transform.position - pos).magnitude <= 5) return false;
        }
        spawnAble = true;
        
        return spawnAble;
    }

    private IEnumerator spawning = null;
    private void StartSpawning()
    {
        spawning = SpawnBodyCo();
        StartCoroutine(spawning);
    }

    private void StopSpawning()
    {
        StopCoroutine(spawning);
        while(bodys.Count == 0)
        {
            bodys.RemoveAt(0);
        }
    }

    private IEnumerator SpawnBodyCo()
    {
        while(true)
        {
            yield return null;

            if(bodys.Count == 0)
            {
                SpawnBody();
                continue;
            }

            for(int i = 0; i < bodys.Count; i++)
            {
                if(bodys[i].GetComponent<BodyController>().isOnPlayer)
                {
                    bodys.Remove(bodys[i]);
                }
            }
        }
    }
}
