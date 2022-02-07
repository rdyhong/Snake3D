using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    private Transform outPosition;
    private void Awake()
    {
        outPosition = this.transform.GetChild(1).transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = outPosition.position;
        }
    }
}
