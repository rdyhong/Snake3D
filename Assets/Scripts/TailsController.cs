using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour
{
    private BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum { get; private set; }

    private void Configue()
    {
        col = gameObject.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        Configue();
    }
    private void OnTriggerEnter(Collider other)
    {
        SnakeController sc = other.transform.GetComponent<SnakeController>();
        TailsController tc = other.transform.GetComponent<TailsController>();
        if (isOnPlayer || tc != null) return;
        if (other.transform.tag == "Player")
        {
            if (sc.isDead) return;
            
            sc.GrowSnake(gameObject ,transform.position);
        }
    }

    public void StartGet(int idx)
    {
        isOnPlayer = true;
        GameManager.instance.SpawnMeal();
        gameObject.GetComponent<BoxCollider>().enabled = false;
        teamNum = idx;
        PlayerGet(idx);
    }
    void PlayerGet(int idx)
    {
        if (idx == 1)
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
        }
        
    }
}
