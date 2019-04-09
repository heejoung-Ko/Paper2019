using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpPower = 5.0f;

    public float minX = -360.0f;
    public float maxX = 360.0f;
    public float minY = -60.0f;
    public float maxY = 90.0f;
    public float sensX = 100.0f;
    public float sensY = 50.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private float mouseRotationX = 0.0f;
    private float mouseRotationY = 0.0f;

    private Transform playerCamera = null;
    private Vector3 moveVector = new Vector3(0, 0, 0);
    private Rigidbody rigidbody;
    private float verticalMove = 0.0f;
    private float horizontalMove = 0.0f;
    private bool isJump = false;

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
        moveVector.Set(horizontalMove, 0, verticalMove);
        moveVector = moveVector.normalized * speed * Time.deltaTime;
        transform.Translate(moveVector);
    }

    private void PlayerJump()
    {
        if (!isJump) return;
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isJump = false;
    }
}
