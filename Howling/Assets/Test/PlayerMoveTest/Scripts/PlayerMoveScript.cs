using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    public float speed = 10.0f;

    public float minX = -360.0f;
    public float maxX = 360.0f;

    public float sensX = 100.0f;

    private float rotationX = 0.0f;

    private bool isKeyDown = false;
    private float mouseRotationX = 0.0f;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            isKeyDown = true;
            if (Input.GetAxis("Vertical") > 0)
            {
                // (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                // (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                this.transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                // (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                this.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                // (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                this.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }
        else
        {
            isKeyDown = false;
        }

        mouseRotationX = Input.GetAxis("Mouse X");
        if ((mouseRotationX > float.MinValue) || (mouseRotationX < -float.MinValue))
        {
            rotationX += mouseRotationX * sensX * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, minX, maxX);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
    }
}
