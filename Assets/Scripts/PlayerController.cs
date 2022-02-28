using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public enum State{ Ready, Stable, Dead }
    public State state = State.Stable;

    public delegate void OnDead();
    public OnDead onDead;

    public int teamNum { get; private set; }
    private Rigidbody rb;
    public PlayerSkill ps { get; private set; }
    private PlayerInput playerInput;

    private GameObject skin;
    private GameObject bodyPosition;

    private float moveSpeed = 5;
    private float steerSpeed = 130;
    private float bodySpeed = 5;
    private int Gap = 10;

    // Lists
    public List<GameObject> BodyParts = new List<GameObject>();
    public List<Vector3> PositionsHistory = new List<Vector3>();
    
    private void Init()
    {
        skin = GameObject.Find("Skin");

        // First body position
        bodyPosition = Instantiate(new GameObject("BodyPosition"), transform.position, transform.rotation, this.transform);
        bodyPosition.transform.localPosition = new Vector3(0, 0, -0.2f);

        // GetComponent
        rb = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        ps = transform.GetComponent<PlayerSkill>();
        
        state = State.Ready;
    }

    private void Awake()
    {
        Init();
        
        GameManager.instance.gameOver += Dead;
        teamNum = 1;
    }

    private void FixedUpdate()
    {
        if(state == State.Stable)
        {
            Move();
            SkinRotation();
        }
    }
    
    private void Move()
    {
        RaycastHit hitHeight;

        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Steer
        transform.Rotate(Vector3.up * playerInput.dirX * steerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, bodyPosition.transform.position);

        // Move body parts
        int index = 0;
        for(int i = 0; i < BodyParts.Count; i++)
        {
            GameObject body = BodyParts[i];

            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path

            if((point - body.transform.position).magnitude > 15f)
            {
                body.transform.position = point;
            }
            else
            {
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * bodySpeed * Time.deltaTime;
            }
            // Vector3 moveDirection = point - body.transform.position;
            // body.transform.position += moveDirection * BodySpeed * Time.deltaTime;
            
            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }

        // Height

        float maxDistance = 50f;

        Vector3 rayStartPos = transform.position;

        bool isHit = Physics.Raycast(rayStartPos, -Vector3.up, out hitHeight, maxDistance);
        if(!isHit)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (1.5f * Time.deltaTime), transform.position.z);
        }

        float targetHeight = hitHeight.point.y + 1f;

        if(transform.position.y <= targetHeight) // Up
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetHeight, 50f * Time.deltaTime), transform.position.z);
            // transform.position = new Vector3(transform.position.x, transform.position.y + 5f * Time.deltaTime, transform.position.z);
        }
        else // Down
        {
            // transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetHeightB, 1f * Time.deltaTime), transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f * Time.deltaTime, transform.position.z);
        }
    }
    
    private void SkinRotation()
    {        
        RaycastHit hitF;
        RaycastHit hitB;

        float maxDistance = 50f;

        Vector3 rayStartPosF = transform.position + (transform.forward / 2);
        rayStartPosF.y = transform.position.y - 0.1f;

        Vector3 rayStartPosB = transform.position + (-transform.forward / 2);
        rayStartPosB.y = transform.position.y - 0.1f;

        bool isFrontHit = Physics.Raycast(rayStartPosF, -Vector3.up, out hitF, maxDistance);
        bool isBackHit = Physics.Raycast(rayStartPosB, -Vector3.up, out hitB, maxDistance);
        
        if(!isFrontHit || !isBackHit) // Ray to Bottom
        {
            skin.transform.position = new Vector3(transform.position.x, transform.position.y - (1f * Time.deltaTime), transform.position.z);
            return;
        }

        // Debug.DrawRay(rayMiddlePos, -Vector3.up * maxDistance, Color.green);
        Debug.DrawRay(rayStartPosF, -Vector3.up * maxDistance, Color.blue);
        Debug.DrawRay(rayStartPosB, -Vector3.up * maxDistance, Color.red);

        //     ===== Head up down Rotation =====

        // Clamp Rotation
        Vector3 hitPointF = hitF.point;
        Vector3 hitPointB = hitB.point;
        if(hitPointF.y < hitPointB.y - 0.6f) hitPointF.y = hitPointB.y - 0.6f;
        else if(hitPointF.y > hitPointB.y + 0.6f) hitPointF.y = hitPointB.y + 0.6f;

        transform.forward = Vector3.Lerp(transform.forward ,hitPointF - hitPointB, 0.5f);

        float height = (rayStartPosF - hitF.point).magnitude;
        float heightB = (rayStartPosB - hitB.point).magnitude;
        
        float targetHeight = hitF.point.y + 1f; // 1 magnitude form ground
        float targetHeightB = hitB.point.y + 1f; // 1 magnitude form ground
    }

    // Physics.SphereCast (transform.position, transform.lossyScale.x / 2, transform.forward, out hit, maxDistance);

    public void GetBody(GameObject obj)
    {
        // Add in list
        BodyParts.Add(obj);

        //Set body
        BodyController bc = obj.GetComponent<BodyController>();
        bc.GetBodyLink(this.gameObject);

        UIManager.instance.AddScoreAndCoin(50, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state == State.Dead || !GameManager.instance.isPlaying) return;
        

        if(collision.transform.tag == "Body")
        {
            Debug.Log("Tail Enter");
        }
        else 
        {
            if(collision.transform.tag == "Debris" || collision.transform.tag == "Item" || 
            collision.transform.tag == "EquipItem") return;

            GameManager.instance.gameOver();
        }
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
