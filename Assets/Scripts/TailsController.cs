using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailsController : MonoBehaviour
{

    // Debris
    public GameObject drPrefab;
    float m_force = 0f;
    Vector3 m_offset = Vector3.zero;

    //
    private BoxCollider col;
    public bool isOnPlayer = false;
    public int teamNum { get; private set; }

    private void Configue()
    {
        col = gameObject.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        Configue();
    }
    private void OnTriggerEnter(Collider other)
    {
        SnakeController sc = other.transform.GetComponent<SnakeController>();
        TailsController tc = other.transform.GetComponent<TailsController>();
        if (isOnPlayer || tc != null)
        {
            return;
        }
        if (other.transform.tag == "Player")
        {
            if (sc.isDead) return;
            GameManager.instance.SpawnMeal();
            sc.GrowSnake(transform.position);
            Destroy(gameObject);
        }
    }
    int idxx;
    public void StartGet(int idx)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        idxx = idx;
        Invoke("PlayerGet", 1.1f);
    }
    void PlayerGet()
    {
        if (idxx == 1)
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
        }
        isOnPlayer = true;
    }

    public void SetTeamNum(int idx)
    {
        teamNum = idx;
    }

    //public void Explosion()
    //{
    //    Debug.Log("Explosion Active");
    //    m_force = 60f;
    //    m_offset = new Vector3(0, -0.25f, 0);
    //    GameObject debris = ObjectPool.GetDebris();
    //    debris.transform.position = transform.position;
    //    //GameObject t_clone = Instantiate(drPrefab, transform.position, Quaternion.identity);
    //    Rigidbody[] t_rb = debris.GetComponentsInChildren<Rigidbody>();
    //    for (int i = 0; i < t_rb.Length; i++)
    //    {
    //        t_rb[i].AddExplosionForce(m_force, transform.position + m_offset, 10f);
    //    }
    //    //gameObject.SetActive(false);

    //}
}
