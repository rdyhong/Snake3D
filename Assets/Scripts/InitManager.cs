using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InitManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(InitCo());
    }

    IEnumerator InitCo()
    {
        GameObject canvas = GameObject.Find("Canvas");
        Image img = canvas.transform.Find("CoverImage").GetComponent<Image>();
        
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);

        yield return null;

        yield return new WaitForSeconds(1f);

        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 0), 1f);

        yield return new WaitUntil(() => img.color.a == 0);

        yield return new WaitForSeconds(3f);
        
        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 1), 1f);

        yield return new WaitUntil(() => img.color.a == 1);

        LoadingManager.LoadScene("Lobby");
    }
}
