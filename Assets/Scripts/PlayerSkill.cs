using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerSkill : MonoBehaviour
{
    public enum SkillState{ Ready, Jump, Throw, Shield }
    public SkillState skillState = SkillState.Ready;

    private SphereCollider col;

    private void Init()
    {
        col = gameObject.GetComponent<SphereCollider>();
    }
    
    private void Awake()
    {
        Init();
    }

    private Sequence jumperSeq;
    public void Skill_Jumper()
    {
        skillState = SkillState.Jump;

        col.enabled = false;
        jumperSeq = DOTween.Sequence();
        Vector3 targetScale = new Vector3(0.05f, 2f, 0.05f);
        Vector3 curScale = new Vector3(1, 1, 1);
        jumperSeq.Append(transform.DOScale(targetScale, 0.01f));
        jumperSeq.Append(transform.DOScale(curScale, 0.3f)).OnComplete(() => 
        {
            col.enabled = true;
        });

        transform.position = transform.position + (transform.forward * 5);

        skillState = SkillState.Ready;
    }

    public GameObject objToThrow;

    public void Skill_ThrowObj()
    {
        skillState = SkillState.Throw;

        Vector3 startPos = transform.position + Vector3.up * 2;
        Vector3 target = transform.forward * 2 + transform.up * 0.5f;

        GameObject obj = Instantiate(objToThrow, startPos, Quaternion.identity);
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        obj.transform.localScale = new Vector3(0,0,0);
        obj.transform.DOScale(new Vector3(1,1,1), 1f);
        objRb.AddForce(target * 30, ForceMode.Impulse);

        skillState = SkillState.Ready;
    }

    public GameObject shield;
    private GameObject[] shields;
    Vector3 shieldCurruntScale = Vector3.zero;

    public void Skill_Shield()
    {
        skillState = SkillState.Shield;
        shieldCurruntScale = shield.transform.localScale;
        shields = new GameObject[3];
        for(int i = 0; i < shields.Length; i++)
        {
            shields[i] = Instantiate(shield, transform.position + transform.forward * 1.5f, Quaternion.Euler(transform.forward));
            shields[i].transform.localScale = new Vector3(0,0,0);
            
        }
        // GameObject obj = Instantiate(shield, transform.position + transform.forward * 1.5f, Quaternion.Euler(transform.forward));
        StartCoroutine(ShieldActiveCo());
    }
    private IEnumerator ShieldActiveCo()
    {
        Vector3[] pos = new Vector3[shields.Length];
        Vector3[] rot = new Vector3[shields.Length];

        float timer = 0;

        for(int i = 0; i < shields.Length; i++)
        {
            shields[i].transform.DOScale(shieldCurruntScale, 1f);
        }

        while(true)
        {
            timer += Time.deltaTime;

            pos[0] = transform.position + transform.forward;
            pos[1] = transform.position + -transform.right;
            pos[2] = transform.position + transform.right;
            rot[0] = transform.forward;
            rot[1] = -transform.right;
            rot[2] = transform.right;

            for(int i = 0; i < shields.Length; i++)
            {
                shields[i].transform.position = pos[i];
                shields[i].transform.forward = rot[i];
            }

            if(timer >= 5)
            {
                for(int i = 0; i < shields.Length; i++)
                {
                    shields[i].transform.DOScale(new Vector3(0,0,0), 1f);
                }

                break;
            }
            yield return null;
        }
            
        while(true)
        {
            pos[0] = transform.position + transform.forward;
            pos[1] = transform.position + -transform.right;
            pos[2] = transform.position + transform.right;
            rot[0] = transform.forward;
            rot[1] = -transform.right;
            rot[2] = transform.right;

            for(int i = 0; i < shields.Length; i++)
            {
                shields[i].transform.position = pos[i];
                shields[i].transform.forward = rot[i];
            }

            if(shields[shields.Length - 1].transform.localScale == new Vector3(0,0,0)) break;

            yield return null;
        }            

        for(int i = 0; i < shields.Length; i++)
        {
            Destroy(shields[i]);
        }

        skillState = SkillState.Ready;
    }
}
