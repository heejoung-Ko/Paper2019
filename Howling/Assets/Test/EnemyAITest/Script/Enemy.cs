using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적 상태 
    public enum EnemyState { idle, walking, trace, escape, attack, die };
    public EnemyState state = EnemyState.idle;

    public Transform target = null;   // 현재 타겟으로 잡고 있는 객체

    public float cognizanceDist = 15.0f;   // 인식 범위 
    public float attackDist = 10.0f;       // 공격 범위

    public float velocity = 0.0f;           // 속도
    public float walkAcc = 0.02f;           // 걸을 때 가속도
    public float runAcc = 0.1f;             // 뛸 때 가속도 (추적, 도주 상태 일 때)

    public float actionTime = 0.0f;
    public float nowTime = 0.0f;

    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(state == EnemyState.trace)
        {
            // 타겟과의 거리 계산
            float distance = Vector3.Distance(target.position, transform.position);
            // 타겟에 닿았을 때
            if (distance <= 0.5f)
            {
                // 타겟의 위치 랜덤화
                // 본 게임에서는 어택 상태로 변환
                target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
                NextAction();
                state = EnemyState.idle;
                target = null;
                return;
            }
            direction = (target.position - transform.position).normalized; // 타겟으로 향하는 방향 
            velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산
            this.transform.position = new Vector3(  transform.position.x + direction.x * velocity,
                                                    transform.position.y,
                                                    transform.position.z + direction.z * velocity    );
            return;
        }
        else if(state == EnemyState.escape)
        {
            Vector3 direction = (target.position - transform.position).normalized; // 타겟으로 향하는 방향 
            velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산
            this.transform.position = new Vector3(  transform.position.x + (-direction.x) * velocity,
                                                    transform.position.y,
                                                    transform.position.z + (-direction.z) * velocity    );
            return;
        }
        else if(state == EnemyState.idle)
        {
            nowTime += Time.deltaTime;
            if (nowTime >= actionTime)
            {
                NextAction();
                state = EnemyState.walking;
                this.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                direction = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                return;
            }
        }
        else if(state == EnemyState.walking)
        {
            velocity = velocity + walkAcc * Time.deltaTime;                // 속도 계산
            float newX = transform.position.x + direction.x * velocity;
            newX = Mathf.Clamp(newX, -5.0f, 5.0f);
            float newZ = transform.position.z + direction.z * velocity;
            newZ = Mathf.Clamp(newZ, -5.0f, 5.0f);
            this.transform.position = new Vector3(newX, transform.position.y, newZ);
            nowTime += Time.deltaTime;
            if (nowTime >= actionTime)
            {
                NextAction();
                state = EnemyState.idle;
            }
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null)
        {
            if (other.tag == "target")
            {
                state = EnemyState.trace;
                target = other.transform;
            }
        }
    }

    void NextAction()
    {
        actionTime = Random.value * 5;
        nowTime = 0.0f;
    }
}
