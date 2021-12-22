using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public int teamNum { get; private set; }
    private SphereCollider col;
    private Rigidbody rb;
    private PlayerInput playerInput;
    Transform tailPos;
    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 8;

    // References
    //public GameObject BodyPrefab;
    public bool isDead;

    // Lists
    [SerializeField]
    private List<GameObject> BodyParts = new List<GameObject>();
    
    private List<Vector3> PositionsHistory = new List<Vector3>();

    private void Configue()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<SphereCollider>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        tailPos = transform.Find("TailPos");
        Gap = 8;
        isDead = false;
    }
    void Start()
    {
        Configue();

        SetTeamNum(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetFreeTail();
        }
        else if(Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerDead();
        }
    }

    private void Move()
    {
        // Move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // Steer
        transform.Rotate(Vector3.up * playerInput.dirX * SteerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, tailPos.position);

        // Move body parts
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }
    }

    public void GrowSnake(GameObject obj ,Vector3 pos)
    {
        // add it to the list
        BodyParts.Add(obj);
        TailsController tc = obj.GetComponent<TailsController>();
        tc.StartGet(teamNum);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        //PlayerDead();

        if (collision.transform.tag == "Tails")
        {
            TailsController tc = collision.transform.GetComponent<TailsController>();
            if (teamNum == tc.teamNum) return;
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

    public void SetTeamNum(int idx)
    {
        teamNum = idx;
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
            StartCoroutine("BoomCoroutine");
        }
        
    }

    IEnumerator BoomCoroutine()
    {
        foreach (var body in BodyParts)
        {
            GameObject obj = ObjectPool.GetDebris();
            obj.GetComponent<DebrisScript>().Expolsive(body);
            body.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void GetFreeTail()
    {
        GameObject[] tails = GameObject.FindGameObjectsWithTag("Tails");
        for(int i = 0; i < tails.Length; i++)
        {
            TailsController tc = tails[i].GetComponent<TailsController>();
            if (tc.isOnPlayer) continue;
            else
            {
                GrowSnake(tails[i], tails[i].transform.position);
            }
        }
        
        
    }

}