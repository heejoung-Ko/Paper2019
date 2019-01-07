using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

    public int timer = 0;
    public GameObject girl;
    public float speed = 3.0f;

	// Use this for initialization
	void Start () {
        Debug.Log("초기화가 이루어졌습니다~");
        girl = GameObject.Find("girl");
	}
	
	// Update is called once per frame
	void Update () {
        timer++;
        Debug.Log(timer + "번째 업데이트");

        // Vector3 : 3차원 벡터
        // forward : 앞으로
        // Time.deltaTime : 컴퓨터 성능에 관계없이 일정하게 움직여주기 위해 사용함
        //       girl.GetComponent<Transform>().Translate(Vector3.forward * speed * Time.deltaTime);
        // 같은 코드 : this.transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 키보드 입력으로 이동시키기
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(0, 0, -speed * Time.deltaTime);
        }
    }
}
