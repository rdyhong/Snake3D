using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour
{
    public BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum;
    public Color color;

    private void Configue()
    {
        col = gameObject.GetComponent<BoxCollider>();
        color = gameObject.GetComponent<Renderer>().material.color;
        teamNum = 99;
    }
    private void Awake()
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
            
            sc.GrowSnake(gameObject);
        }
    }
}
