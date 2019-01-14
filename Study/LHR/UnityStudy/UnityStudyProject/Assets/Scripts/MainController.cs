using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public int timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("초기화가 이루어졌습니다.");
    }

    // Update is called once per frame
    void Update()
    {
        timer += 1;
        Debug.Log(timer + "번째 업데이트");
    }
}
