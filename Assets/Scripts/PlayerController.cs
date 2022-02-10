using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public enum State{ Stable, Dead }
    public State state = State.Stable;
    

    public delegate void OnDead();
    public OnDead onDead;

    public int teamNum { get; private set; }
    private Rigidbody rb;
    private SphereCollider col;
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
    
    private void Init()
    {
        // First body position
        bodyPosition = Instantiate(new GameObject("BodyPosition"), transform.position, transform.rotation, this.transform);
        bodyPosition.transform.localPosition = new Vector3(0, 0, -0.5f);

        // GetComponent
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<SphereCollider>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        
        state = State.Stable;
    }

    private void Awake()
    {
        Init();
        
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

            if((point - body.transform.position).magnitude > 15f)
            {
                body.transform.position = point;
            }
            else
            {
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * BodySpeed * Time.deltaTime;
            }
            // Vector3 moveDirection = point - body.transform.position;
            // body.transform.position += moveDirection * BodySpeed * Time.deltaTime;
            
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
        rayStartPos.y = transform.position.y - 0.1f;
        Vector3 rayStartPosB = transform.position + (-transform.forward / 2);
        rayStartPosB.y = transform.position.y - 0.1f;

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

        // Clamp Rotation
        Vector3 hitPointF = hit.point;
        Vector3 hitPointB = hitB.point;
        if(hitPointF.y < hitPointB.y - 0.6f) hitPointF.y = hitPointB.y - 0.6f;
        else if(hitPointF.y > hitPointB.y + 0.6f) hitPointF.y = hitPointB.y + 0.6f;

        transform.forward = Vector3.Lerp(transform.forward ,hitPointF - hitPointB, 0.5f);

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

        //     ===== Test =====
        ob1.transform.position = hit.point;
        ob2.transform.position = hitB.point;
        //     ================
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
        if(collision.transform.tag == "Debris" || collision.transform.tag == "Item" || 
        collision.transform.tag == "EquipItem") return;

        if(collision.transform.tag == "Body")
        {
            Debug.Log("Tail Enter");
        }
        else 
        {
            Debug.Log(collision.transform.name);
            collision.transform.position = collision.transform.position + collision.transform.up * 10;
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


    // ===== Test =====
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


    public enum SkillState{ Ready, Jump, Throw, Shield }
    public SkillState skillState = SkillState.Ready;

    private Sequence jumperSeq;
    public void Skill_Jumper()
    {
        skillState = SkillState.Jump;

        col.enabled = false;
        jumperSeq = DOTween.Sequence();
        Vector3 targetScale = new Vector3(0.05f, 2f, 0.05f);
        Vector3 curScale = new Vector3(1, 1, 1);
        jumperSeq.Append(transform.DOScale(targetScale, 0.01f));
        jumperSeq.Append(transform.DOScale(curScale, 0.3f)).OnComplete(() => 
        {
            col.enabled = true;
        });

        transform.position = transform.position + (transform.forward * 5);

        skillState = SkillState.Ready;
    }

    public GameObject objToThrow;

    public void Skill_ThrowObj()
    {
        skillState = SkillState.Throw;

        Vector3 startPos = transform.position + Vector3.up * 2;
        Vector3 target = transform.forward * 2 + transform.up * 1.2f;

        GameObject obj = Instantiate(objToThrow, startPos, Quaternion.identity);
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        obj.transform.localScale = new Vector3(0,0,0);
        obj.transform.DOScale(new Vector3(1,1,1), 1f);
        objRb.AddForce(target * 25, ForceMode.Impulse);

        skillState = SkillState.Ready;
    }

    public GameObject shield;
    private GameObject[] shields;
    Vector3 shieldCurruntScale = Vector3.zero;

    public void Skill_Shield()
    {
        skillState = SkillState.Shield;
        shieldCurruntScale = shield.transform.localScale;
        shields = new GameObject[3];
        for(int i = 0; i < shields.Length; i++)
        {
            shields[i] = Instantiate(shield, transform.position + transform.forward * 1.5f, Quaternion.Euler(transform.forward));
            shields[i].transform.localScale = new Vector3(0,0,0);
            shields[i].transform.DOScale(shieldCurruntScale, 1f);
        }
        // GameObject obj = Instantiate(shield, transform.position + transform.forward * 1.5f, Quaternion.Euler(transform.forward));
        StartCoroutine(ShieldActive());
    }
    private IEnumerator ShieldActive()
    {
        // bool isactive = true;
        Vector3[] pos = new Vector3[shields.Length];
        Vector3[] rot = new Vector3[shields.Length];
        float timer = 0;
        while(true)
        {
            timer += Time.deltaTime;

            pos[0] = transform.position + transform.forward;
            pos[1] = transform.position + -transform.right;
            pos[2] = transform.position + transform.right;
            rot[0] = transform.forward;
            rot[1] = -transform.right;
            rot[2] = transform.right;

            for(int i = 0; i < shields.Length; i++)
            {
                shields[i].transform.position = pos[i];
                shields[i].transform.forward = rot[i];
            }

            if(timer >= 5)
            {
                timer = -100f; // For play Dotween once
                for(int i = 0; i < shields.Length; i++)
                {
                    shields[i].transform.DOScale(new Vector3(0,0,0), 1f);
                }
            }
            if(shields[0].transform.localScale == new Vector3(0,0,0)) break;
            yield return null;
        }
        yield return new WaitUntil(() => shields[0].transform.localScale == new Vector3(0,0,0));
        for(int i = 0; i < shields.Length; i++)
        {
            Destroy(shields[i]);
        }
        skillState = SkillState.Ready;
    }
    // ==================
}