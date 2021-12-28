using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private BoxCollider col;
    private Vector3 targetPos;
    private Renderer rdr;

    [SerializeField]
    private float rotSpeed;
    private void Awake() 
    {
        col = gameObject.GetComponent<BoxCollider>();
        rdr = gameObject.GetComponent<Renderer>();
        targetPos = transform.position + new Vector3(0, 2.5f, 0);
    }
    private void Start() {
        rotSpeed = 5f;
    }
    private void FixedUpdate() 
    {
        Rotate();
    }
    void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, rotSpeed));
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Player")
        {
            SnakeController sc = other.transform.GetComponent<SnakeController>();
            if(sc.isDead) return;
            Taken();
        }
    }

    private void Taken()
    {
        col.enabled = false;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        Color c = rdr.material.color;
        rotSpeed = 100f;
        while(c.a > 0)
        {
            rotSpeed = Mathf.Lerp(rotSpeed, 0f, Time.deltaTime * 3f);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
            c = new Color(c.r, c.g, c.b, c.a - 0.8f * Time.deltaTime);
            rdr.material.color = c;
            if(c.a <= 0) transform.position = new Vector3(0 ,-100f, 0);
            yield return null;
        }
        Destroy(gameObject);
    }
}
