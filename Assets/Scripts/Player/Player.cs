using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    //玩家移动速度
    public float speed;
    //X轴的输入
    private float inputX;
    //Y轴输入
    private float inputY;
    //XY移动
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MoveMent(); //使用刚体需要在FixUpdate中调用
    }

    /// <summary>
    /// 获取玩家输入情况
    /// </summary>
    private void PlayerInput() 
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //XY同时输入的时候，修正斜向移动速度
        if (inputX != 0 && inputY != 0) 
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }
        movementInput = new Vector2(inputX, inputY);
    }

    private void MoveMent() 
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
}
