using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour
{
    public BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum;
    public Renderer ren;
    public GameObject headPlayer = null;

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
        
        TailsController tc = other.transform.GetComponent<TailsController>();
        
        string tag = other.transform.tag;
        switch(tag)
        {
            case "Player":
            
            PlayerController pc = other.transform.GetComponent<PlayerController>();
            if (pc.isDead) return;
            if(isOnPlayer)
            {
                if(pc.BodyParts[0] == this.gameObject) return;
                pc.PlayerDead();
                return;
            }
            else
            {
                pc.GrowSnake(gameObject);
            }
            break;

            case "Vehicle":
            PlayerController m_pc = headPlayer.transform.GetComponent<PlayerController>();
            if(m_pc.isDead) return;
            m_pc.RemoveHitBody(this.gameObject);

            break;


        } 
    }
    public void LateColActive() => Invoke("LateCol", 1.0f);
    void LateCol() => col.enabled = true;

}
