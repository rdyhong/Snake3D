using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviour
{
    public GameObject debris;
    Rigidbody[] drRb;

    float m_force = 0f;
    Vector3 m_offset = Vector3.zero;
    void Start()
    {
        drRb = GetComponentsInChildren<Rigidbody>();
    }

    public void Expolsive(Vector3 pos)
    {
        m_force = 60f;
        m_offset = new Vector3(0, -0.25f, 0);
        transform.position = pos;
        for (int i = 0; i < drRb.Length; i++)
        {
            drRb[i].AddExplosionForce(m_force, pos + m_offset, 10f);
        }
    }
}
