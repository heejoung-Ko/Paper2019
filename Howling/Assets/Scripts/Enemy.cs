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

    public float attackDist = 10.0f;            // 공격 범위

    public float velocity = 0.0f;               // 속도
    public float walkAcc = 0.01f;               // 걸을 때 가속도
    public float runAcc = 0.02f;                // 뛸 때 가속도 (추적, 도주 상태 일 때)

    public static float walkMaxVel = 3.0f;      // 걸을 때 최고 속도
    public static float runMaxVel = 3.0f;      // 뛸 때 최고 속도

    public static float keepTraceTime = 5.0f;   // 타겟이 인식 범위 밖으로 나갔을 때 추적 상태를 유지하는 시간
    public static float keepEscapeTime = 5.0f;  // 타겟이 인식 범위 밖으로 나갔을 때 도주 상태를 유지하는 시간

    public float nextStateTime = 0.0f;          // 다음 랜덤 상태까지 걸리는 총 시간
    public float nowStateTime = 0.0f;           // 현재 기본 상태(idle, walk)에서 보낸 시간

    public Vector3 direction;                   // 이동 방향

    public Rigidbody rigidbody;

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
            // if (distance <= 1.0f)
            // {
            //     // 타겟의 위치 랜덤화
            //     // 본 게임에서는 어택 상태로 변환
            //     target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
            //     ChageNextState();
            //     state = EnemyState.idle;
            //     target = null;
            //     return;
            // }

            direction = (target.position - transform.position).normalized; // 타겟으로 향하는 방향
            direction.y = 0;

            velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산
            velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);            

            Move();

            // 타겟이 인식 범위 밖에 있을 경우
            if (nextStateTime == keepTraceTime)
            {
                nowStateTime += Time.deltaTime;
                if(nowStateTime >= nextStateTime)
                {
                    ChangeNextState();
                    state = EnemyState.walk;
                    target = null;
                }
            }

            return;
        }
        else if(state == EnemyState.escape)
        {
            direction = (target.position - transform.position).normalized;  // 타겟으로 향하는 방향 
            direction.y = 0;
            direction *= -1;                                                // 타겟으로 향하는 역방향으로 전환

            velocity = velocity + runAcc * Time.deltaTime;                  // 속도 계산
            velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);

            Move();

            // 타겟이 인식 범위 밖에 있을 경우
            if (nextStateTime == keepTraceTime)
            {
                nowStateTime += Time.deltaTime;
                if (nowStateTime >= nextStateTime)
                {
                    ChangeNextState();
                    state = EnemyState.walk;
                    target = null;
                }
            }

            return;
        }
        else if(state == EnemyState.idle)
        {
            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChangeNextState();
                state = EnemyState.walk;
                direction = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                return;
            }
        }
        else if(state == EnemyState.walk)
        {
            velocity = velocity + walkAcc * Time.deltaTime;                // 속도 계산
            velocity = Mathf.Clamp(velocity, 0.0f, walkMaxVel);

            Move();

            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChangeNextState();
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
                ChangeNextState();
                nextStateTime = 0.0f;
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (state == EnemyState.trace)
            nextStateTime = keepTraceTime;
        else if (state == EnemyState.escape)
            nextStateTime = keepEscapeTime;
    }

    private void Move()
    {
        // rigidbody.AddForce(direction * velocity * Time.deltaTime, ForceMode.Force);
        this.transform.position = new Vector3(this.transform.position.x + direction.x * velocity * Time.deltaTime,
                                                    this.transform.position.y,
                                                    this.transform.position.z + direction.z * velocity * Time.deltaTime);

        Quaternion newRotation = Quaternion.LookRotation(direction);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);
    }

    void ChangeNextState()
    {
        // 2 ~ 5 초 행동하고 다음 행동을 수행
        nextStateTime = 2 + Random.value * 3;
        nowStateTime = 0.0f;
        velocity = 0.0f;        // 속도 초기화
    }
}
