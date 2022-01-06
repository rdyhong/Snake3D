using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController pc;
    public float dirX { get; private set; }

    public bool btnLInput = false;
    public bool btnRInput = false;

    public bool isMobile;
    private void Awake()
    {
        pc = gameObject.GetComponent<PlayerController>();
        isMobile = false;
    }

    private void Update()
    {
        if (pc.isDead) return;
        
        //Mobile Input
        if(isMobile)
        {
            TouchInput();
        }
        //PC Input
        else
        {
            KeyboardInput();
        }

        //Test Input
        if (Input.GetKeyDown(KeyCode.Space)) // Key Space , Add near body
        {
            pc.GetFreeTail();
        }
        else if(Input.GetKeyDown(KeyCode.Backspace)) // Key BackSpace , Player Die
        {
            pc.PlayerDead();
        }
        else if(Input.GetKeyDown(KeyCode.P))  // Key P , Remove Body index 0
        {
            GameObject obj = pc.BodyParts[0];
            pc.RemoveHitBody(obj);
        }
        else if(Input.GetKeyDown(KeyCode.M)) // Key M , Switch Touch, Keyboard
        {
            switch(isMobile)
            {
                case true:
                isMobile = false;
                break;

                case false:
                isMobile = true;
                break;
            }
        }
    }

    private void KeyboardInput()
    {
        dirX = Input.GetAxis("Horizontal");
    }
    private void TouchInput()
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


    public void ButtonLInput() => btnLInput = true;
    public void ButtonRInput() => btnRInput = true;

    public void ButtonUp()
    {
        btnRInput = false;
        btnLInput = false;
    }
}
