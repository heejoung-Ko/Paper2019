using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class Enemy : MonoBehaviour
    {
        public int m_EnemyNumber = 1;

        // 적 상태 
        public enum EnemyState { idle, walk, trace, escape, attack, die };
        public EnemyState state = EnemyState.idle;

        // 현재 타겟으로 설정된 객체
        GameObject target = null;

        float attackDist = 1.5f;            // 공격 범위

        float velocity = 0.0f;               // 속도
        float walkAcc = 0.6f;               // 걸을 때 가속도
        float runAcc = 1.3f;                // 뛸 때 가속도 (추적, 도주 상태 일 때)

        static float walkMaxVel = 6.0f;      // 걸을 때 최고 속도
        static float runMaxVel = 13.0f;      // 뛸 때 최고 속도

        static float keepTraceTime = 5.0f;   // 타겟이 인식 범위 밖으로 나갔을 때 추적 상태를 유지하는 시간
        static float keepEscapeTime = 5.0f;  // 타겟이 인식 범위 밖으로 나갔을 때 도주 상태를 유지하는 시간

        float nextStateTime = 0.0f;          // 다음 랜덤 상태까지 걸리는 총 시간
        float nowStateTime = 0.0f;           // 현재 기본 상태(idle, walk)에서 보낸 시간

        Vector3 direction;                   // 이동 방향

        int hp = 10;         // 체력
        int atk = 5;        // 공격력

        // Update is called once per frame
        void FixedUpdate()
        {
            if (state == EnemyState.trace)
            {
                Trace();
                return;
            }
            else if (state == EnemyState.escape)
            {
                Escape();
                return;
            }
            else if (state == EnemyState.idle)
            {
                Idle();
                return;
            }
            else if (state == EnemyState.walk)
            {
                Walk();
                return;
            }
            else if (state == EnemyState.attack)
            {
                Attack();
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
                    target = other.gameObject;
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

        void Idle()
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

        void Walk()
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

        void Trace()
        {
            // 타겟과의 거리 계산
            float distance = Vector3.Distance(target.transform.position, transform.position);

            // 타겟이 공격범위 안에 들어왔을 때
            if (distance <= attackDist)
            {
                state = EnemyState.attack;
                nowStateTime = 0f;
                nextStateTime = 2f;
                velocity = 0f;
                return;
            }

            direction = (target.transform.position - transform.position).normalized; // 타겟으로 향하는 방향
            direction.y = 0;

            velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산
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
        }

        void Escape()
        {
            direction = (target.transform.position - transform.position).normalized;  // 타겟으로 향하는 방향 
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
        }

        void Attack()
        {
            nowStateTime += Time.deltaTime;
            if (nowStateTime <= 0.5f)
            { 
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(this.transform.rotation.x - 90, 0, this.transform.rotation.z - 90));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 10.0f);
                return;
            }
            else if(nowStateTime <= 1.5f)
            {
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(this.transform.rotation.x + 180, 0, this.transform.rotation.z + 180));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 10.0f);
            }
            else if(nowStateTime <= nextStateTime)
            {
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(this.transform.rotation.x - 90, 0, this.transform.rotation.z - 90));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 10.0f);
            }
            else
            {
                state = EnemyState.trace;
                ChangeNextState();
            }
        }
        
        void Die()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if(state == EnemyState.attack)
            {
                target.transform.Find("Canvas").Find("Status").GetComponent<StatusController>().HitEnemy(atk);
            }
        }
    }
}