using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    // float time;
    public static float maxTime = 180f;

    // Start is called before the first frame update
    void Start()
    {
        // time = 0f;
        Destroy(gameObject, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        // time += Time.deltaTime;
        // 
        // if (time > maxTime)
        //     Destroy(gameObject);
    }
}
