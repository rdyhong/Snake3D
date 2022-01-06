using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleScript : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private GameObject target;
    public GameObject[] wheels;

    private void Awake() {
        target = transform.Find("TargetPos").gameObject;
        startPos = transform.position;
        endPos = target.transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate() 
    {
        target.transform.position = endPos;
        Move();
        Rotation();
    }

    void Move()
    {
        transform.position += (target.transform.position - transform.position).normalized * 5f * Time.deltaTime;
        if((target.transform.position - transform.position).magnitude <= 2f)
        {
            transform.position = startPos;
        }
        // Rotate each wheel
        foreach(GameObject wheel in wheels)
        {
            wheel.transform.Rotate(new Vector3(4f, 0f, 0f));
        }
    }
    void Rotation()
    {
        transform.forward = target.transform.position - transform.position;
    }
}
