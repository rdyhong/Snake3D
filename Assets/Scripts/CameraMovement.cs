using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private CinemachineVirtualCamera followCam;
    private Vector3 deadLastPos = Vector3.zero;
    private Transform player;
    private Transform camPos;

    void Start()
    {
        followCam = gameObject.GetComponent<CinemachineVirtualCamera>();
        player = FindObjectOfType<PlayerController>().gameObject.transform;
        camPos = player.transform.Find("CamPos");
        followCam.Follow = camPos.transform;
        followCam.LookAt = camPos.transform;
    }

    private void Update() {
        if(player.GetComponent<PlayerController>().state != PlayerController.State.Dead) return;
        DeadCamRotate();
    }
    
    private void DeadCamRotate()
    {
        Vector3 dir = player.GetComponent<PlayerController>().PositionsHistory[5] - camPos.transform.position;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        camPos.transform.rotation = rot;
    }
}
