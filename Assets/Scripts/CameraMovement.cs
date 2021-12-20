using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private CinemachineVirtualCamera followCam;
    public Transform pPlay;
    public Transform pDead;
    void Start()
    {
        followCam = gameObject.GetComponent<CinemachineVirtualCamera>();
        followCam.Follow = pPlay.transform;
        followCam.LookAt = pPlay.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    IsPlayerDead(true);
        //}
        //else if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    IsPlayerDead(false);
        //}
    }

    public void IsPlayerDead(bool isdead)
    {
        if (isdead)
        {
            followCam.Follow = pDead.transform;
            followCam.LookAt = pDead.transform;
        }
        else
        {
            followCam.Follow = pPlay.transform;
            followCam.LookAt = pPlay.transform;
        }
    }
}
