using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CautionScript : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        StartCoroutine(Blinking());
    }

    void Update()
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, -0.8f, player.transform.position.z);
        transform.forward = transform.position - playerPos;
    }

    private IEnumerator Blinking()
    {
        // bool isEnd = false;
        Renderer[] rdr = transform.GetComponentsInChildren<Renderer>();
        for(int i = 0; i < 3; i++)
        {
            foreach(Renderer m_rdr in rdr)
            {
                Material mat = m_rdr.material;
                mat.DOFade(0f, 0.2f);
            }
            yield return new WaitUntil(() => rdr[0].material.color.a == 0);
            foreach(Renderer m_rdr in rdr)
            {
                Material mat = m_rdr.material;
                mat.DOFade(1f, 0.2f);
            }
            yield return new WaitUntil(() => rdr[0].material.color.a == 1);
        }
        // if(isEnd) 
        foreach(Renderer m_rdr in rdr)
        {
            Material mat = m_rdr.material;
            mat.DOFade(0f, 0.2f);
        }
        yield return new WaitUntil(() => rdr[0].material.color.a == 0);

        Destroy(this.gameObject);
    }
}
