using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Start()
    {
        OnLoadScene();
    }

    private void OnLoadScene()
    {
        StartCoroutine(InitCo());
    }
    IEnumerator InitCo()
    {
        GameObject imgObj = GameObject.Find("Canvas").transform.Find("CoverImage").gameObject;
        Image img = imgObj.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        
        yield return new WaitForSeconds(1f);

        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 0), 1f).OnComplete(() => {
            imgObj.SetActive(false);
        });
    }

    public void PlayButtonEvent()
    {
        StartCoroutine(PlayButtonCo());
    }
    IEnumerator PlayButtonCo()
    {
        GameObject imgObj = GameObject.Find("Canvas").transform.Find("CoverImage").gameObject;
        Image img = imgObj.GetComponent<Image>();
        imgObj.SetActive(true);
        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 1), 1f);

        yield return new WaitUntil(() => img.color.a == 1);
        LoadingManager.LoadScene("Main");
    }
}
