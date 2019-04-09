﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private Transform playerCamera = null;
    private Rigidbody rigidbody;

    // WASD move
    private float velocity = 0.0f;
    public static float walkAcc = 0.015f;
    public static float runAcc = 0.025f;
    private static float walkMaxVel = 5.0f;
    private static float runMaxVel = 10.0f;
    private Vector3 moveVector = new Vector3(0, 0, 0);
    private float verticalMove = 0.0f;
    private float horizontalMove = 0.0f;

    // jump
    private float jumpPower = 5.0f;
    private bool isJump = false;

    // mouse rotation
    private float minX = -360.0f;
    private float maxX = 360.0f;
    private float minY = -60.0f;
    private float maxY = 90.0f;
    private float sensX = 100.0f;
    private float sensY = 50.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private float mouseRotationX = 0.0f;
    private float mouseRotationY = 0.0f;

    void Awake()
    {
        playerCamera = GameObject.Find("Main Camera").transform;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        mouseRotationX = Input.GetAxis("Mouse X");
        mouseRotationY = Input.GetAxis("Mouse Y");
        verticalMove = Input.GetAxis("Vertical");
        horizontalMove = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
            isJump = true;
    }

    void LateUpdate()
    {
        PlayerRotation();
        PlayerMove();
        PlayerJump();
    }

    private void PlayerRotation()
    {
        // mouse rotation
        if ((mouseRotationX > Mathf.Epsilon) || (mouseRotationX < -Mathf.Epsilon))
        {
            rotationX += mouseRotationX * sensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
        if ((mouseRotationY > Mathf.Epsilon) || (mouseRotationY < -Mathf.Epsilon))
        {
            rotationY += mouseRotationY * sensY * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            playerCamera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
    }

    private void PlayerMove()
    {
        // WASD/방향키 move
        if (((horizontalMove < Mathf.Epsilon) && (horizontalMove > -Mathf.Epsilon))
        && ((verticalMove < Mathf.Epsilon) && (verticalMove > -Mathf.Epsilon)))
        {
            velocity = 0.0f;
            moveVector.Set(0, 0, 0);
            transform.Translate(moveVector);
            return;
        }

        velocity = velocity + walkAcc * Time.deltaTime;
        velocity = Mathf.Clamp(velocity, 0.0f, walkMaxVel);

        moveVector.Set(horizontalMove, 0, verticalMove);
        moveVector = moveVector.normalized * velocity;
        transform.Translate(moveVector);
    }

    private void PlayerJump()
    {
        if (!isJump) return;
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isJump = false;
    }
}
