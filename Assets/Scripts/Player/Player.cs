using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    //����ƶ��ٶ�
    public float speed;
    //X�������
    private float inputX;
    //Y������
    private float inputY;
    //XY�ƶ�
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
        MoveMent(); //ʹ�ø�����Ҫ��FixUpdate�е���
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    private void PlayerInput() 
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //XYͬʱ�����ʱ������б���ƶ��ٶ�
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
