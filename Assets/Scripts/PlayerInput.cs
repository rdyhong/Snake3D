using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private SnakeController sc;
    public float dirX { get; private set; }
    private void Awake()
    {
        sc = gameObject.GetComponent<SnakeController>();
    }
    private void Update()
    {
        if (sc.isDead) return;
        KeyboardInput();
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
}
