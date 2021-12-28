using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviour
{
    private Rigidbody[] rb;
    private BoxCollider[] col;
    private Renderer[] rdr;
    private float m_force = 60f;
    private Vector3 m_pos = Vector3.zero;
    private void Awake()
    {
        rb = this.gameObject.GetComponentsInChildren<Rigidbody>();
        col = this.gameObject.GetComponentsInChildren<BoxCollider>();
        rdr = gameObject.GetComponentsInChildren<Renderer>();
        m_force = 140;
        m_pos = new Vector3(0, -0.2f, 0);
    }
    
    public void Expolsive(GameObject obj, Color m_color)
    {
        transform.position = obj.transform.position;
        transform.rotation = obj.transform.rotation;
        gameObject.SetActive(true);
        for (int i = 0; i < rb.Length; i++)
        {
            rdr[i].material.color = m_color;
            if (Random.Range(0, 2) == 0) continue;
            rb[i].AddExplosionForce(m_force, m_pos, 40f);
        }
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        //Remove Colider after Addforce
        yield return new WaitForSeconds(0.1f);
        // for(int i = 0; i < col.Length; i++)
        // {
        //     col[i].enabled = true;
        // }
        
        //Fade
        while(rdr[rdr.Length - 1].material.color.a > 0)
        {
            for(int i = 0; i < rdr.Length; i++)
            {
                rdr[i].material.color = new Color(rdr[i].material.color.r, rdr[i].material.color.g, rdr[i].material.color.b, rdr[i].material.color.a - 0.4f * Time.deltaTime);
            }
            yield return null;
        }
        DebrisPool.ReturnDebris(gameObject);
    }
}
