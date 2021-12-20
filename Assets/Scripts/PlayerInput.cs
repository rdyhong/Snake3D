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
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        if (sc.isDead) return;
        dirX = Input.GetAxis("Horizontal");
    }
}
