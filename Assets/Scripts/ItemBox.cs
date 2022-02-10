using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemBox : MonoBehaviour
{
    private GameObject coin = null;
    List<GameObject> coins = new List<GameObject>();

    private int selectedIdx = 0;
    private void Awake()
    {
        coin = Resources.Load<GameObject>("Coin");

        SelectRandomItem();
    }

    private void SelectRandomItem()
    {
        int idx = Random.Range(0,2);
        selectedIdx = idx;
        switch(idx)
        {
            case 0:
            int count = Random.Range(7, 15);
            for(int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(coin, transform.position, Quaternion.identity);
                coins.Add(obj);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 365f), 0f));
                obj.GetComponent<BoxCollider>().enabled = false;
                obj.SetActive(false);
            }
            break;

            case 1:
            
            break;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            BoxOpen();
        }
    }

    // private Sequence sq;
    private void BoxOpen()
    {
        // sq = DOTween.Sequence();
        if(selectedIdx == 0)
        {
            this.gameObject.SetActive(false);

            for(int i = 0; i < coins.Count; i++)
            {
                coins[i].SetActive(true);
                Vector3 randDir = transform.position + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
                coins[i].transform.DOMove(randDir, 1f);
                coins[i].transform.DOLocalMoveY(2, 0.5f).SetLoops(2, LoopType.Yoyo);
            }
            Invoke("DestroyThis", 1);
        }
        else{
            Debug.Log("It's Empty!@!@!");
            Destroy(this.gameObject);
        }
    }

    private void DestroyThis()
    {
        for(int i = 0; i < coins.Count; i++)
        {
            coins[i].GetComponent<BoxCollider>().enabled = true;
        }
        Destroy(this.gameObject);
    }
}
