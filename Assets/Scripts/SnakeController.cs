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
    public int Gap = 10;
    public Material mat;
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

    public void GrowSnake(GameObject obj)
    {
        // add it to the list
        BodyParts.Add(obj);

        //Set body
        TailsController tc = obj.GetComponent<TailsController>();
        tc.isOnPlayer = true;
        tc.col.enabled = false;
        tc.teamNum = teamNum;

        GameManager.instance.SpawnMeal();

        //Remove Collider at first body
        if (tc.teamNum == 1) tc.col.enabled = false;
        else tc.col.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if(collision.transform.tag == "Debris") return;
        if(collision.transform.tag == "Item") return;
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
            Color m_color = body.GetComponent<TailsController>().color;
            obj.GetComponent<DebrisScript>().Expolsive(body, m_color);
            body.SetActive(false);
            yield return new WaitForSeconds(0.1f);
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