using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private BoxCollider col;
    private Vector3 targetPos;
    private Renderer rdr;

    private float rotSpeed = 5f;
    private void Awake() 
    {
        col = this.gameObject.GetComponent<BoxCollider>();
        rdr = this.gameObject.GetComponent<Renderer>();
        targetPos = transform.position + new Vector3(0, 2.5f, 0);
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
            PlayerController sc = other.transform.GetComponent<PlayerController>();
            if(sc.state == PlayerController.State.Dead) return;
            Taken();
        }
    }

    private void Taken()
    {
        col.enabled = false;
        UIManager.instance.AddScoreAndCoin(10, 50);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color c = rdr.material.color;
        rotSpeed = 100f;
        while(c.a > 0)
        {
            rotSpeed = Mathf.Lerp(rotSpeed, 0f, Time.deltaTime * 3f); // change rotation speed rotSpeed to 0 while fadeout
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
            c = new Color(c.r, c.g, c.b, c.a - 0.8f * Time.deltaTime);
            rdr.material.color = c;
            if(c.a <= 0) transform.position = new Vector3(0 ,-100f, 0);
            yield return null;
        }
        Destroy(gameObject);
    }
}
