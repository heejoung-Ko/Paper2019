using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    float time = 0f;

    Vector3 rotation = new Vector3(0f, 0f, 0f);

    float velocity = 5;

    Vector3 originPosition;

    int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        rotation.x = Random.Range(-1, 1);
        rotation.z = Random.Range(-1, 1);
        Mathf.Clamp(rotation.x, -1, 1);
        Mathf.Clamp(rotation.z, -1, 1);

        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time > 5)
        {
            rotation.x = Random.Range(-1, 1);
            rotation.z = Random.Range(-1, 1);
            time = 0f;
            Mathf.Clamp(rotation.x, -1, 1);
            Mathf.Clamp(rotation.z, -1, 1);
            num++;
        }

        if(num > 10)
        {
            num = 0;
            transform.position = originPosition;
        }

        this.transform.position = new Vector3(this.transform.position.x + rotation.x * velocity * Time.deltaTime ,
                                                  this.transform.position.y,
                                                  this.transform.position.z + rotation.z * velocity * Time.deltaTime);

        Quaternion newRotation = Quaternion.LookRotation(rotation);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);
    }
}
