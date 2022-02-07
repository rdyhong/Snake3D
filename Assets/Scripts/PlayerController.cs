using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State{ Stable, Jump, Dead }
    public State state = State.Stable;

    // public delegate void Dead();
    // public Dead onDead;

    public int teamNum { get; private set; }
    // private SphereCollider col;
    private Rigidbody rb;
    private PlayerInput playerInput;
    // Settings
    private float MoveSpeed = 5;
    private float SteerSpeed = 130;
    private float BodySpeed = 5;
    private int Gap = 10;
    private GameObject bodyPosition;

    // Lists
    public List<GameObject> BodyParts = new List<GameObject>();
    public List<Vector3> PositionsHistory = new List<Vector3>();
    
    private void Configue()
    {
        // First body position
        bodyPosition = Instantiate(new GameObject("BodyPosition"), transform.position, transform.rotation, this.transform);
        bodyPosition.transform.localPosition = new Vector3(0, 0, -0.5f);

        //
        // Vector3 baseHeight = transform.position + (-Vector3.forward / 2) + -Vector3.up;
        // Instantiate(new GameObject("Temp"), baseHeight, transform.rotation, this.transform);
        //

        rb = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        state = State.Stable;
    }

    private void Awake()
    {
        Configue();
        
        GameManager.instance.gameOver += Dead;
        teamNum = 1;
    }

    private void FixedUpdate()
    {
        if(state == State.Dead) return;
        Move();
        SetHeight();
    }
    
    private void Move()
    {
        // Move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // Steer
        transform.Rotate(transform.up * playerInput.dirX * SteerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, bodyPosition.transform.position);

        // Move body parts
        int index = 0;
        for(int i = 0; i < BodyParts.Count; i++)
        {
            GameObject body = BodyParts[i];

            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path
            if((point - body.transform.position).magnitude > 10f)
            {
                body.transform.position = point;
            }
            else
            {
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * BodySpeed * Time.deltaTime;
            }
            
            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }
    }


    //     ===== Test =====
    public GameObject ob1;
    public GameObject ob2;
    //     ================

    private RaycastHit hit;
    private RaycastHit hitB;
    private void SetHeight()
    {
        if(state != State.Stable) return;
        
        float maxDistance = 50f;

        Vector3 rayStartPos = transform.position + (transform.forward / 2);
        rayStartPos.y = transform.position.y - 0.5f;
        Vector3 rayStartPosB = transform.position + (-transform.forward / 2);
        rayStartPosB.y = transform.position.y - 0.5f;

        bool isFrontHit = Physics.Raycast(rayStartPos, -Vector3.up, out hit, maxDistance);
        bool isBackHit = Physics.Raycast(rayStartPosB, -Vector3.up, out hitB, maxDistance);

        if(!isFrontHit || !isBackHit) // Ray to Bottom
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (2f * Time.deltaTime), transform.position.z);
            return;
        }
        Debug.DrawRay(rayStartPos, -Vector3.up * maxDistance, Color.blue);
        Debug.DrawRay(rayStartPosB, -Vector3.up * maxDistance, Color.red);

        //     ===== Head up down Rotation =====

        Vector3 hitPointF = hit.point;
        Vector3 hitPointB = hitB.point;
        
        if(hitPointF.y < hitPointB.y - 0.3f) hitPointF.y = hitPointB.y - 0.3f;

        transform.forward = Vector3.Lerp(transform.forward ,hitPointF - hitPointB, 0.1f);

        //     ===== Test =====
        ob1.transform.position = hit.point;
        ob2.transform.position = hitB.point;
        //     ================

        float height = (rayStartPos - hit.point).magnitude;
        float heightB = (rayStartPosB - hitB.point).magnitude;
        
        float targetHeight = hit.point.y + 1f; // 1 magnitude form ground
        float targetHeightB = hitB.point.y + 1f; // 1 magnitude form ground

        if(heightB <= targetHeightB) // When Player Up
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetHeightB, 50f * Time.deltaTime), transform.position.z);
        }
        else // When Player Falling
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetHeightB, 1f * Time.deltaTime), transform.position.z);
        }
    }

    public void GetBody(GameObject obj)
    {
        // add it to the list
        BodyParts.Add(obj);
        //Set body
        BodyController bc = obj.GetComponent<BodyController>();
        bc.GetBodyLink(this.gameObject);

        UIManager.instance.AddScoreAndCoin(50, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state == State.Dead || !GameManager.instance.isPlaying) return;
        if(collision.transform.tag == "Debris" || collision.transform.tag == "Item") return;
        if(collision.transform.tag == "Body")
        {
            Debug.Log("Tail Enter");
        }
        else GameManager.instance.gameOver();
    }

    private void Dead()
    {
        state = State.Dead;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // Explosive under, behind of head
        rb.AddExplosionForce(3000f, transform.position + new Vector3(0, -1f, 0) - transform.forward, 50f);

        // Start bodys explosive routine
        if (BodyParts.Count != 0)
        {
            StartCoroutine(BoomCoroutine());
        }
    }

    private IEnumerator BoomCoroutine()
    {
        // foreach (var body in BodyParts)
        for(int i = 0; i < BodyParts.Count; i++)
        {
            GameObject body = BodyParts[i];

            GameObject obj = DebrisPool.GetDebris();

            obj.GetComponent<DebrisScript>().Expolsive(body);
            body.SetActive(false);
            Destroy(body);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RemoveHitBody(GameObject hit)
    {
        // foreach(GameObject obj in BodyParts)
        for(int i = 0; i < BodyParts.Count; i++)
        {
            GameObject obj = BodyParts[i];

            if(hit == BodyParts[i])
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


    // For Test
    public void GetFreeTail()
    {
        GameObject[] tails = GameObject.FindGameObjectsWithTag("Body");
        for(int i = 0; i < tails.Length; i++)
        {
            BodyController tc = tails[i].GetComponent<BodyController>();
            
            if (tc.isOnPlayer) continue;
            else GetBody(tails[i]);
        }
    }
}