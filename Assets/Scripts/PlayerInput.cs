using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    private PlayerController pc;
    public float dirX { get; private set; }

    public bool btnLInput = false;
    public bool btnRInput = false;

    private bool isMobile = false;
    private void Awake()
    {
        pc = gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (pc.state == PlayerController.State.Dead) return;
        
        if(isMobile) TouchInput();   //Mobile Input
        else KeyboardInput();   //PC Input


        //Test Input
        if (Input.GetKeyDown(KeyCode.Space)) // Key Space - Add near body
        {
            pc.GetFreeTail();
        }
        else if(Input.GetKeyDown(KeyCode.Backspace)) // Key BackSpace - Player Die
        {
            GameManager.instance.gameOver();
        }
        else if(Input.GetKeyDown(KeyCode.P))  // Key P - Remove Body index 0
        {
            GameObject obj = pc.BodyParts[0];
            pc.RemoveHitBody(obj);
        }
        else if(Input.GetKeyDown(KeyCode.M)) // Key M - Switch Mobile & Keyboard
        {
            if(isMobile) isMobile = false;
            else isMobile = true;
        }
        
        // Skill
        if(pc.ps.skillState == PlayerSkill.SkillState.Ready)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                pc.ps.Skill_Jumper();
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                pc.ps.Skill_ThrowObj();
            }
            else if(Input.GetKeyDown(KeyCode.Z))
            {
                pc.ps.Skill_Shield();
            }
        }
    }

    private void KeyboardInput() => dirX = Input.GetAxis("Horizontal");

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
