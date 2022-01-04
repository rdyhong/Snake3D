using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour
{
    public BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum;
    public Renderer ren;

    private void Configue()
    {
        col = gameObject.GetComponent<BoxCollider>();
        ren = gameObject.GetComponent<Renderer>();
        teamNum = 99;
    }
    private void Awake()
    {
        Configue();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.transform.GetComponent<PlayerController>();
        TailsController tc = other.transform.GetComponent<TailsController>();
        // if (isOnPlayer || tc != null) return;
        if (other.transform.tag == "Player")
        {
            if (pc.isDead) return;
            if(isOnPlayer)
            {
                pc.PlayerDead();
                return;
            }
            pc.GrowSnake(gameObject);
        }
    }
    public void LateColActive() => Invoke("LateCol", 1.0f);
    void LateCol() => col.enabled = true;

}
