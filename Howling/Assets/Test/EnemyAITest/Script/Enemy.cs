using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적 상태 
    public enum EnemyState { idle, walk, trace, escape, attack, die };
    public EnemyState state = EnemyState.idle;

    // 현재 타겟으로 설정된 객체
    public Transform target = null;

    public float attackDist = 10.0f;        // 공격 범위

    public float velocity = 0.0f;           // 속도
    public float walkAcc = 0.01f;           // 걸을 때 가속도
    public float runAcc = 0.02f;             // 뛸 때 가속도 (추적, 도주 상태 일 때)

    public float nextStateTime = 0.0f;      // 다음 랜덤 상태까지 걸리는 총 시간
    public float nowStateTime = 0.0f;       // 현재 기본 상태(idle, walk)에서 보낸 시간

    public Vector3 direction;               // 이동 방향

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
                ChageNextState();
                state = EnemyState.idle;
                target = null;
                return;
            }

            direction = (target.position - transform.position).normalized; // 타겟으로 향하는 방향 
            direction.y = 0;

            velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산

            Move();

            return;
        }
        else if(state == EnemyState.escape)
        {
            direction = (target.position - transform.position).normalized; // 타겟으로 향하는 방향 

            velocity = velocity + runAcc * Time.deltaTime;                  // 속도 계산

            Move();

            return;
        }
        else if(state == EnemyState.idle)
        {
            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChageNextState();
                state = EnemyState.walk;
                direction = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                return;
            }
        }
        else if(state == EnemyState.walk)
        {
            velocity = velocity + walkAcc * Time.deltaTime;                // 속도 계산

            Move();

            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChageNextState();
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

    private void Move()
    {
        this.transform.position = new Vector3(this.transform.position.x + direction.x * velocity,
                                                    this.transform.position.y,
                                                    this.transform.position.z + direction.z * velocity);
        this.transform.rotation = Quaternion.LookRotation(this.transform.forward + direction * Time.deltaTime * 5);
    }

    void ChageNextState()
    {
        nextStateTime = Random.value * 5;
        nowStateTime = 0.0f;
    }
}
