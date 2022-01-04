using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController sc;
    public float dirX { get; private set; }

    public bool btnLInput = false;
    public bool btnRInput = false;

    public bool isMobile;
    private void Awake()
    {
        sc = gameObject.GetComponent<PlayerController>();
        isMobile = false;
    }

    private void Update()
    {
        if (sc.isDead) return;
        
        Debug.Log(dirX);

        //Mobile Input
        if(isMobile)
        {
            if(btnLInput)
            {
                if(dirX < -0.99f) dirX = -1;
                else dirX = Mathf.Lerp(dirX, -1f, 5f * Time.deltaTime);
            }
            else if(btnRInput)
            {
                if(dirX > 0.99f) dirX = 1f;
                else dirX = Mathf.Lerp(dirX, 1f, 5f * Time.deltaTime);
            }
            else
            {
                if(dirX < 0.01f && dirX > -0.01f) dirX = 0f;
                else dirX = Mathf.Lerp(dirX, 0f, 5f * Time.deltaTime);
            }
        }
        //PC Input
        else
        {
            KeyboardInput();
        }

        //Test Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sc.GetFreeTail();
        }
        else if(Input.GetKeyDown(KeyCode.Backspace))
        {
            sc.PlayerDead();
        }
    }

    private void KeyboardInput()
    {
        dirX = Input.GetAxis("Horizontal");
    }
    public void ButtonLInput() => btnLInput = true;
    public void ButtonRInput() => btnRInput = true;

    public void ButtonUp()
    {
        btnRInput = false;
        btnLInput = false;
    }
}
