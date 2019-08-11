﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public int m_EnemyNumber = 1;
    private Animator animator;

    // 적 상태 
    public enum EnemyState { idle, walk, trace, escape, attack, hit, die };
    public EnemyState state = EnemyState.idle;
    public EnemyState oldState;
    public EnemyType type;

    // 현재 타겟으로 설정된 객체
    GameObject target = null;

    float attackDist = 1.5f;            // 공격 범위

    private const float atkPos = 1f;
    private float atkRange = 1f;
    public LayerMask targetMask;

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

    public int hp;         // 체력
    public int atk;        // 공격력

    public float invincibleTime = 1f; // 무적 시간
    public float currentInvincibleTime = 0f;

    bool isDead;
    bool isAttack;

    Quaternion oldRotation;

    public GameObject dropItem;
    private EnemiesManager enemiesManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        isDead = false;
        isAttack = false;
        //targetMask = 1 << LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == EnemyState.trace)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isMoving", false);
            Trace();
            return;
        }
        else if (state == EnemyState.escape)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isMoving", false);
            Escape();
            return;
        }
        else if (state == EnemyState.idle)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);
            Idle();
            return;
        }
        else if (state == EnemyState.walk)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", true);
            Walk();
            return;
        }
        else if (state == EnemyState.attack)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);

            animator.SetTrigger("attckTrigger");
            Attack();
            return;
        }
        else if (state == EnemyState.hit)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);

            animator.SetTrigger("hitTrigger");
            Hit();
        }
        else if (state == EnemyState.die)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);

            animator.SetTrigger("dieTrigger");
            Die();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null)
        {
            if (other.tag == "target" || other.tag == "agent")
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
            animator.SetBool("isMoving", false);
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
            oldRotation = transform.rotation;
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
        if (!isAttack)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, targetMask);
            foreach (Collider t in targets)
            {
                //Debug.Log(t.tag);
                if (t.gameObject.CompareTag("target"))
                {
                    GameObject.Find("UIManager").transform.GetChild(0).Find("Status").GetComponent<StatusController>().HitEnemy(atk);
                }
                else if (t.gameObject.CompareTag("agent"))
                {
                    WolfAgent wolf = t.GetComponent<WolfAgent>();
                    wolf.Hp -= atk;
                }

                isAttack = true;
            }
        }

        nowStateTime += Time.deltaTime;
        if (nowStateTime >= nextStateTime)
        {
            state = EnemyState.trace;
            ChangeNextState();
            isAttack = false;
        }
        return;

        //if (nowStateTime <= 0.5f)
        //{
        //    Quaternion rotation = Quaternion.identity;
        //    rotation.eulerAngles = new Vector3(0, 30f, 0);
        //    Quaternion newRotation = oldRotation * rotation;
        //    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);
        //    return;
        //}
        //else if (nowStateTime <= 1.5f)
        //{
        //    Quaternion rotation = Quaternion.identity;
        //    rotation.eulerAngles = new Vector3(0, -60f, 0);
        //    Quaternion newRotation = oldRotation * rotation;
        //    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);

        //}
        //else if (nowStateTime <= nextStateTime)
        //{
        //    Quaternion rotation = Quaternion.identity;
        //    rotation.eulerAngles = new Vector3(0, 0, 0);
        //    Quaternion newRotation = oldRotation * rotation;
        //    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);
        //}
        //else
        //{
        //    state = EnemyState.trace;
        //    ChangeNextState();
        //    isAttack = false;
        //}
    }

    void Hit()
    {
        currentInvincibleTime += Time.deltaTime;
        if (currentInvincibleTime > invincibleTime)
        {
            state = oldState;
            currentInvincibleTime = 0f;
        }
    }

    void Die()
    {
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0, 0, 90);
        Quaternion newRotation = oldRotation * rotation;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime);

        if (!isDead)
        {
            enemiesManager.Die(this);
            isDead = true;
        }
    }

    public void DecreaseHp(int cnt)
    {
        if (state == EnemyState.die || state == EnemyState.hit)
            return;
        {
            hp -= cnt;
            oldState = state;
            state = EnemyState.hit;
            velocity = 0;
            Debug.Log("데미지: " + cnt);

            if (hp <= 0)
            {
                Debug.Log("주겄당!!");
                state = EnemyState.die;
                oldRotation = transform.rotation;
                animator.SetTrigger("dieTrigger");
            }
        }
        //Debug.Log("enemy hp: " + hp);
    }

    void DropItem()
    {
        Debug.Log("아이템 뿌린당!!!");
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
}
