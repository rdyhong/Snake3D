using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private void Start() 
    {
        OnSpawn(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(headPlayer != null)
        {
            if(headPlayer.GetComponent<PlayerController>().state == PlayerController.State.Dead)
            {
                return;
            }
        }

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
        OnSpawn(false);

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


    private void OnSpawn(bool isSpawn)
    {
        if(isSpawn)
        {
            RaycastHit hit;
            bool isHit = Physics.Raycast(transform.position, -Vector3.up, out hit, 100f);

            if(isHit)
            {
                transform.position = hit.point + new Vector3(0, 1, 0);
            }
            else
            {
                Destroy(this.gameObject);
            }

            co = OnSpawnCo();
            StartCoroutine(co);
        }
        else
        {
            DOTween.Kill(this);
            StopCoroutine(co);
        }
    }
    private IEnumerator co;
    private IEnumerator OnSpawnCo()
    {
        Vector3 curruntPos = transform.position;
        Vector3 targetPos = transform.position + new Vector3(0, 1, 0);
        // bool isUp = false;
        while(true)
        {
            transform.Rotate(Vector3.up * 50f * Time.deltaTime);

            // if(isUp) transform.DOMove(targetPos, 1);
            // else transform.DOMove(curruntPos, 1);

            // if((transform.position -targetPos).magnitude < 0.05f) isUp = false;
            // else if((transform.position - curruntPos).magnitude < 0.05f) isUp = true;

            yield return null;
        }
        // Sequence myC = DOTween.Sequence();
    }
}
