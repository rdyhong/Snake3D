using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviour
{
    private Rigidbody[] rb;
    private BoxCollider[] col;
    private Renderer[] rdr;
    private Vector3[] debrisLocalPos;
    private Quaternion[] debrisLocalRot;

    private void Config()
    {
        rb = this.gameObject.GetComponentsInChildren<Rigidbody>();
        col = this.gameObject.GetComponentsInChildren<BoxCollider>();
        rdr = gameObject.GetComponentsInChildren<Renderer>();

        debrisLocalPos = new Vector3[transform.childCount];
        debrisLocalRot = new Quaternion[transform.childCount];
        
        SetEachDebrisLocalPos(true);
    }

    private void Awake()
    {
        Config();
    }

    private void SetEachDebrisLocalPos(bool isSetting)
    {
        if(isSetting)
        {
            for(int i = 0; i < debrisLocalPos.Length; i++)
            {
                debrisLocalPos[i] = transform.GetChild(i).localPosition;
                debrisLocalRot[i] = transform.GetChild(i).localRotation;
            }
        }
        else
        {
            for(int i = 0; i < debrisLocalPos.Length; i++)
            {
                transform.GetChild(i).localPosition = debrisLocalPos[i];
                transform.GetChild(i).localRotation = debrisLocalRot[i];
                rdr[i].material.color = new Color(rdr[i].material.color.a, rdr[i].material.color.g, rdr[i].material.color.b, 1);
            }
        }
    }

    public void Expolsive(GameObject obj)
    {
        transform.position = obj.transform.position;
        transform.rotation = obj.transform.rotation;
        gameObject.SetActive(true);
        Renderer m_ren = obj.GetComponent<Renderer>();
        for (int i = 0; i < rb.Length; i++)
        {
            rdr[i].material = m_ren.material;
            if (Random.Range(0, 2) == 0) continue; // Random AddForce 1/2
            rb[i].AddExplosionForce(150f, new Vector3(0,-0.2f,0), 40f);
        }
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        //Fade out debris
        while(rdr[rdr.Length - 1].material.color.a > 0)
        {
            for(int i = 0; i < rdr.Length; i++)
            {
                rdr[i].material.color = new Color(rdr[i].material.color.r, rdr[i].material.color.g, rdr[i].material.color.b, rdr[i].material.color.a - 0.4f * Time.deltaTime);
            }
            yield return null;
        }
        SetEachDebrisLocalPos(false);
        DebrisPool.ReturnDebris(this.gameObject);
    }
}
