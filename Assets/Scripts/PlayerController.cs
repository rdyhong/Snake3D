using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int teamNum { get; private set; }
    public bool isDead{get; private set;}
    private SphereCollider col;
    private Rigidbody rb;
    private PlayerInput playerInput;
    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 10;
    public GameObject tailPos;
    

    // Lists
    public List<GameObject> BodyParts = new List<GameObject>();
    
    private List<Vector3> PositionsHistory = new List<Vector3>();
    
    private void Configue()
    {
        tailPos = transform.Find("TailPos").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<SphereCollider>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        Gap = 10;
        isDead = false;
    }
    void Start()
    {
        teamNum = 1;
        Configue();
    }

    private void FixedUpdate() {
        if (isDead) return;
        Move();
    }
    
    private void Move()
    {
        // Move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // Steer
        transform.Rotate(Vector3.up * playerInput.dirX * SteerSpeed * Time.deltaTime);
        // Store position history
        PositionsHistory.Insert(0, tailPos.transform.position);

        // Move body parts
        int index = 0;
        foreach (var body in BodyParts)
        {
            // if(body == BodyParts[0]) body.GetComponent<TailsController>().col.enabled = false;
            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }
    }

    public void GrowSnake(GameObject obj)
    {
        GameManager.instance.SpawnMeal();
        // add it to the list
        BodyParts.Add(obj);
        //Set body
        TailsController tc = obj.GetComponent<TailsController>();
        tc.col.enabled = false;
        tc.isOnPlayer = true;
        tc.teamNum = teamNum;
        tc.headPlayer = this.gameObject;
        tc.LateColActive();

        UIManager.instance.AddScoreAndCoin(50, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if(collision.transform.tag == "Debris" || collision.transform.tag == "Item") return;
        if (collision.transform.tag == "Tails")
        {
            TailsController tc = collision.transform.GetComponent<TailsController>();
            if (tc.isOnPlayer)
            {
                PlayerDead();
            }
        }
        else
        {
            PlayerDead();
        }
    }

    public void PlayerDead()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        isDead = true;
        MoveSpeed = 0;
        SteerSpeed = 0;
        BodySpeed = 0;

        if (BodyParts.Count != 0)
        {
            StartCoroutine(BoomCoroutine());
        }
    }

    IEnumerator BoomCoroutine()
    {
        foreach (var body in BodyParts)
        {
            GameObject obj = DebrisPool.GetDebris();
            obj.GetComponent<DebrisScript>().Expolsive(body);
            body.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RemoveHitBody(GameObject hit)
    {
        foreach(GameObject obj in BodyParts)
        {
            if(hit == obj)
            {
                BodyParts.Remove(obj);
                obj.SetActive(false);
                Destroy(obj);
                GameObject debrisObj = DebrisPool.GetDebris();
                debrisObj.GetComponent<DebrisScript>().Expolsive(obj);
                return;
            }
        }
    }

    public void GetFreeTail()
    {
        GameObject[] tails = GameObject.FindGameObjectsWithTag("Tails");
        for(int i = 0; i < tails.Length; i++)
        {
            TailsController tc = tails[i].GetComponent<TailsController>();
            
            if (tc.isOnPlayer) continue;
            else
            {
                GrowSnake(tails[i]);
            }
        }
    }

}