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
        //followCam.Follow = pPlay.transform;
        //followCam.LookAt = pPlay.transform;
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
