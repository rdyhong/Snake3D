using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleScript : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    void Start()
    {
        startPos = transform.position;
        endPos = new Vector3(-40, -1, -25);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (endPos - transform.position).normalized * 5f * Time.deltaTime;
        if((endPos - transform.position).magnitude <= 0.1f)
        {
            transform.position = startPos;
        }
    }
}
