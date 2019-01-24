using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

    private float speed = 10.0f;
    private float rSpeed = 100.0f;

    private float v;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        v = Input.GetAxis("Vertical");

	    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        { 
            this.transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if(v >= 0)
            {
                this.transform.Rotate(0, -rSpeed * Time.deltaTime, 0);
            }
            else
            {
                this.transform.Rotate(0, rSpeed * Time.deltaTime, 0);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (v >= 0)
            {
                this.transform.Rotate(0, rSpeed * Time.deltaTime, 0);
            } else
            {
                this.transform.Rotate(0, -rSpeed * Time.deltaTime, 0);
            }
        }
    }
}
