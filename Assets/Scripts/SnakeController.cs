using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public int teamNum { get; private set; }
    private SphereCollider col;
    private PlayerInput playerInput;
    Transform tailPos;
    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 8;

    // References
    public GameObject BodyPrefab;
    public bool isDead;

    // Lists
    [SerializeField]
    private List<GameObject> BodyParts = new List<GameObject>();
    
    private List<Vector3> PositionsHistory = new List<Vector3>();

    private void Configue()
    {
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
            GrowSnake(Vector3.zero);
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

    public void GrowSnake(Vector3 pos)
    {
        // Instantiate body instance and
        // add it to the list
        GameObject body = Instantiate(BodyPrefab, pos, Quaternion.identity);
        BodyParts.Add(body);
        TailsController tc = body.GetComponent<TailsController>();
        tc.StartGet(BodyParts.Count);
        tc.SetTeamNum(teamNum);

        Debug.Log(BodyParts.Count);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isDead) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        PlayerDead();
    }

    public void SetTeamNum(int idx)
    {
        teamNum = idx;
    }

    public void PlayerDead()
    {
        isDead = true;
        MoveSpeed = 0;
        SteerSpeed = 0;
        BodySpeed = 0;

        if (BodyParts.Count != 0)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                GameObject dr = ObjectPool.GetDebris();
                DebrisScript drS = dr.GetComponent<DebrisScript>();
                drS.Expolsive(BodyParts[i].transform.position);
            }
            //BodyParts[0].GetComponent<TailsController>().Explosion();
            //}
            //foreach (var body in BodyParts)
            //{
            //body.GetComponent<TailsController>().Explosion();
        }

        
    }

    
}