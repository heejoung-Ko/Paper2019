using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WolfAgent : Agent
{
    public enum WolfState
    {
        Hungry = 1, Died, Normal
    };

    public enum PlayerRelation
    {
        Enemy , Stranger, Friend, Soulmate
    }

    private Rigidbody wolfRigidbody;
    public Transform pivotTransform; // 위치 기준점

    // 타겟이 상태에 따라 유동적으로 바뀌어야함
    public Transform target_food;
    public Transform target_home;

    public float moveForce;
    private bool targetEaten = false;
    private bool dead = false;
    private bool rest = false;

    public float minRange = -10f;
    public float maxRange = 10f;

    int state = (int)WolfState.Normal;

    // 늑대의 스테이터스
    public float hp = 100f;
    public float hungry = 100f;
    public float friendly = 0f;

    // 패널티
    private float penalty = 0f;
    private float hpPenalty = 0f;
    private float hungryPenalty = 0f;
    private float friendlyPenalty = 0f;

    private int hpIncreaseSpeed;
    private int hpRechargeTime;
    private int currentHpRechargeTime;

    private int hungryDecreseTime;
    private int currentHungryDecreaseTime;

    private int hpRewardTime = 0;
    private bool isResetTarget = false;
    private int resetTargetTime = 0;
    private Transform target;

    void Awake()
    {
        wolfRigidbody = GetComponent<Rigidbody>();
    }

    void ResetTarget()
    {
        //Debug.Log("ResetTarget");

        targetEaten = false;
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
        target_food.position = randomPos + pivotTransform.position;

        isResetTarget = false;
        resetTargetTime = 0;
        target_food.gameObject.SetActive(true);
        //Debug.Log(target.position);

    }

    // Agent가 리셋할때 자동으로 실행되는 함수 - Agent를 어떻게 reset할지 작성
    // 1. agnet 게임 오브젝트가 처음 활성화 될 때
    // 2. 에피소드가 끝나고 다음 에피소드가 넘어가서 학습환경을 초기화 할 때
    // 3. 목표를 완수 했을 때
    // 4. 목표를 실패했을 때 (agent가 죽어버린 경우도 포함)
    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;

        //Debug.Log(randomPos);
        //Debug.Log(pivotTransform.position);
        dead = false;
        rest = false;
        state = (int)WolfState.Normal;
        hp = 100f;
        hungry = 100f;

        wolfRigidbody.velocity = Vector3.zero;

        ResetTarget();
    }

    // Agent가 주변을 관측하는 함수 = Agent의 눈과 귀를 만드는 함수
    public override void CollectObservations()
    {
        if (state == (int)WolfState.Normal)
        {
            target = target_food;
            if (isResetTarget) target = target_home;
        }
        else if (state == (int)WolfState.Died)
        {
            target = target_home;
        }
        else if (state == (int)WolfState.Hungry)
        {
            target = target_food;
            if (isResetTarget) target = target_home;
        }

        Vector3 distanceToTarget = target.position - transform.position;

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / maxRange, -1f, 1f));

        distanceToTarget = target.position - transform.position;

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / maxRange, -1f, 1f));

        Vector3 relativePos = transform.position - pivotTransform.position;

        AddVectorObs(Mathf.Clamp(relativePos.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / maxRange, -1f, 1f));

        AddVectorObs(Mathf.Clamp(wolfRigidbody.velocity.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(wolfRigidbody.velocity.z / maxRange, -1f, 1f));
     
        AddVectorObs(Mathf.Clamp(hungry / 100f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(hp / 100f, -1f, 1f));
     
        AddVectorObs(Mathf.Clamp(state / 3, -1, 1));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        //AddReward(-0.001f);

        // status, state 추가될 때 바뀔 내용. 스크립트로 빼자.
        if (state == (int)WolfState.Normal)
        {
            target = target_food;
            if (isResetTarget) target = target_home;
        }
        else if (state == (int)WolfState.Died)
        {
            target = target_home;
        }
        else if (state == (int)WolfState.Hungry)
        {
            target = target_food;
            if (isResetTarget) target = target_home;
        }

        float d1 = (target.position - transform.position).sqrMagnitude;

        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        transform.LookAt(target);
        transform.position = transform.position + new Vector3(horizontalInput, 0f, verticalInput) * moveForce;

        float d2 = (target.position - transform.position).sqrMagnitude;
        if (d1 > d2)
        {
            float distanceReward = (400 - d2) * 0.001f / 4;
            if (distanceReward >= 0) AddReward(distanceReward);
        }
        else
        {
            float distancePenalty = -0.1f;
            AddReward(distancePenalty);
        }

        if (targetEaten)
        {
            if (target_food.gameObject.activeSelf == true)
            {
                Debug.Log("food 먹었다!");

                AddReward(1 + ((100 - hungry) * 0.01f));

                hungry = hungry + 20f;
                hungry = Mathf.Clamp(hungry, 0f, 100f);

                isResetTarget = true;
                target_food.gameObject.SetActive(false);
                //ResetTarget();
            }
        }

        if (rest)
        {
            AddReward(0.01f);
            hpRewardTime++;

            if (hpRewardTime > 200)
            {
                Debug.Log("home 쉬고있다!");
                AddReward(1 + ((100 - hp) * 0.01f));
                hp = hp + 20f;
                hp = Mathf.Clamp(hp, 0f, 100f);
                hpRewardTime = 0;
            }
        }

        if (dead)
        {
            AddReward(-1.0f);
            Done();
        }

        float hpPenalty = -(100 - hp) * 0.001f;
        //float hungryPenalty = -(100 - hungry) * 0.001f;
        AddReward(hpPenalty);

        //  이름, 어떤 값, 위치
        //Monitor.Log("reword", GetCumulativeReward(), this.gameObject.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("food"))
        {
            targetEaten = true;
        }
        else if (collision.collider.CompareTag("home"))
        {
            rest = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("home"))
        {
            rest = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            //   Debug.Log(hungryPenalty);
            dead = true;
        }
    }

    void Update()
    {
        //if (transform.position.x < minRange || transform.position.z < minRange || transform.position.y < -1f)
        //{
        //    Debug.Log("작아서 주금");
        //    dead = true;
        //}
        //else if (transform.position.x > maxRange || transform.position.z > maxRange || transform.position.y > 3f)
        //{
        //    Debug.Log("커서 주금");
        //    dead = true;
        //}

        if (isResetTarget)
        {
            resetTargetTime++;
            if (resetTargetTime > 200) ResetTarget();
        }

        // 상태 바꿔주는 것도 스크립트로 빼자. 
        if (hungry > 0f)
            hungry = hungry - (Time.deltaTime * 0.5f);
        else
        {
            hungry = 0f;
            hp = hp - Time.deltaTime * 0.5f;
        }

        if (hp <= 0f)
        {
            hp = 0f;
            dead = true;
        }

        if (hp <= 50f)
        {
            state = (int)WolfState.Died;
        }
        else if (hungry <= 50f)
        {
            state = (int)WolfState.Hungry;
        }
        else
        {
            state = (int)WolfState.Normal;
        }
    }

}