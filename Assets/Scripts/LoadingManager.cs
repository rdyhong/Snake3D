using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public static string nextScene;

    private GameObject canvas;
    private static string gameVersion = "0.0.1";

    private GameObject coverPanel;
    private GameObject loadingPanel;
    private Text loadingText;
    private Text loadingText2;
    private Text versionText;
    private Image coverPanelImage;

    private void Init()
    {
        canvas = GameObject.Find("Canvas");
        coverPanel = canvas.transform.Find("CoverPanel").gameObject;
        loadingPanel = canvas.transform.Find("LoadingPanel").gameObject;
        loadingText = loadingPanel.transform.Find("LoadingText").GetComponent<Text>();
        loadingText2 = loadingPanel.transform.Find("LoadingText2").GetComponent<Text>();
        versionText = loadingPanel.transform.Find("VersionText").GetComponent<Text>();

        versionText.text = "Ver " + gameVersion;
        coverPanelImage = coverPanel.GetComponent<Image>();
        coverPanelImage.color = new Color(coverPanelImage.color.r, coverPanelImage.color.g, coverPanelImage.color.b, 1);
    }

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Master...");
        
        StartCoroutine(LoadScene());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
    }


    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadScene");
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        Vector3 curPos = loadingText.transform.position;
        Vector3 startPos = curPos + new Vector3(0,200f, 0);
        Vector3 endPos = curPos + new Vector3(0,-200f, 0);

        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 0);
        loadingText2.color = new Color(loadingText2.color.r, loadingText2.color.g, loadingText2.color.b, 0);

        loadingText.transform.position = startPos;
        loadingText2.transform.position = startPos;

        while(true)
        {
            yield return null;

            coverPanelImage.DOColor(new Color(coverPanelImage.color.r, coverPanelImage.color.g, coverPanelImage.color.b, 0), 1f);

            yield return new WaitUntil(() => coverPanelImage.color.a == 0);

            loadingText.text = "지렁이 머리에 안테나 설치중 ...";
            loadingText.DOColor(new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1f), 2f);
            loadingText.transform.DOMoveY(curPos.y, 1.5f);

            yield return new WaitUntil(() => loadingText.transform.position == startPos);
            yield return new WaitForSeconds(2.5f);

            loadingText.DOColor(new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 0), 1.5f);
            loadingText.transform.DOMoveY(endPos.y, 1.5f).OnComplete(() => {
                loadingText.transform.position = startPos;
            });

            if(op.progress >= 0.9f) break;

            loadingText2.text = "땅 속에서 나오는중 ...";
            loadingText2.DOColor(new Color(loadingText2.color.r, loadingText2.color.g, loadingText2.color.b, 1f), 2f);
            loadingText2.transform.DOMoveY(curPos.y, 1.5f);

            yield return new WaitUntil(() => loadingText2.transform.position == startPos);
            yield return new WaitForSeconds(2.5f);

            loadingText2.DOColor(new Color(loadingText2.color.r, loadingText2.color.g, loadingText2.color.b, 0), 1.5f);
            loadingText2.transform.DOMoveY(endPos.y, 1.5f).OnComplete(() => {
                loadingText2.transform.position = startPos;
            });

            if(op.progress >= 0.9f) break;
        }

        Image img = coverPanel.GetComponent<Image>();
        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 1), 1);

        yield return new WaitUntil(() => img.color.a == 1);

        op.allowSceneActivation = true;
    }
}
