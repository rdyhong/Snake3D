using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    // public enum BodyState{ Ground, onPlayer }
    // public BodyState bodyState = BodyState.Ground;
    public BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum;
    // public Renderer ren;
    private GameObject headPlayer = null;

    private void Configue()
    {
        col = gameObject.GetComponent<BoxCollider>();
        // ren = gameObject.GetComponent<Renderer>();
        teamNum = 99;
    }
    private void Awake()
    {
        Configue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(headPlayer != null) if(headPlayer.GetComponent<PlayerController>().state == PlayerController.State.Dead) return;

        string tag = other.transform.tag;

        if(isOnPlayer)
        {
            switch(tag)
            {
                case "Player":
                if(headPlayer.GetComponent<PlayerController>().BodyParts[0] == this.gameObject) return;
                // headPlayer.GetComponent<PlayerController>().PlayerDead();
                GameManager.instance.gameOver();
                break;

                case "Obstacle":
                ForceToContactObstacle(other.gameObject);
                headPlayer.GetComponent<PlayerController>().RemoveHitBody(this.gameObject);
                break;

                default:
                headPlayer.GetComponent<PlayerController>().RemoveHitBody(this.gameObject);
                break;
            }
        }
        else
        {
            switch(tag)
            {
                case "Player":
                PlayerController pc = other.transform.GetComponent<PlayerController>();
                pc.GetBody(this.gameObject);
                break;
                // case "AIPlayer":
                // AIPlayerController aiPc = other.transform.GetComponent<AIPlayerController>();
                // aiPc.GrowSnake(this.gameObject);
                // break;
            }
        }
    }

    public void GetBodyLink(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        col.enabled = false;
        teamNum = pc.teamNum;
        isOnPlayer = true;
        headPlayer = player;
        Invoke("LateCol", 1.0f);
    }
    private void LateCol() => col.enabled = true;

    private void ForceToContactObstacle(GameObject hit)
    {
        Vector3 dir = hit.transform.position - transform.position;
        Rigidbody hitRb = hit.GetComponent<Rigidbody>();
        hitRb.AddForce(dir * 500f);
    }
}
