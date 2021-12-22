using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviour
{
    private Rigidbody[] rb;
    private BoxCollider[] col;
    //private Renderer renderer;

    private float m_force = 60f;
    private Vector3 m_offset = Vector3.zero;
    private void Awake()
    {
        rb = this.gameObject.GetComponentsInChildren<Rigidbody>();
        col = this.gameObject.GetComponentsInChildren<BoxCollider>();
        m_force = 140;
        //m_offset = new Vector3(0, -0.25f, 0);
    }

    public void Expolsive(GameObject obj)
    {
        Material objRenderer = obj.GetComponent<Renderer>().material;
        //renderer.material.SetTexture(objRenderer)

        transform.position = obj.transform.position;
        transform.rotation = obj.transform.rotation;
        m_offset = new Vector3(0, -0.2f, 0);
        gameObject.SetActive(true);
        for (int i = 0; i < rb.Length; i++)
        {
            if (Random.Range(0, 2) == 0) continue;
            rb[i].AddExplosionForce(m_force, m_offset, 40f);
        }
        StartCoroutine("DestroyCoroutine");
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(10f);
        for(int i = 0; i < col.Length; i++)
        {
            col[i].enabled = false;
        }
        yield return new WaitForSeconds(1.5f);
        ObjectPool.ReturnDebris(gameObject);
    }
}
