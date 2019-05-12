using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WolfAgent : Agent
{
    public enum WolfState
    {
        Hungry, Died, Normal
    };

    public enum PlayerRelation
    {
        Enemy, Stranger, Friend, Soulmate
    }

    private Rigidbody wolfRigidbody;
    public Transform pivotTransform; // 위치 기준점

    // 타겟이 상태에 따라 유동적으로 바뀌어야함
    public Transform target;
    public Transform target2;

    public float moveForce = 10f;
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

    void Awake()
    {
        wolfRigidbody = GetComponent<Rigidbody>();
    }

    void ResetTarget()
    {
        //Debug.Log("ResetTarget");

        targetEaten = false;
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
        target.position = randomPos + pivotTransform.position;

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
        Vector3 distanceToTarget = target.position - transform.position;

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / maxRange, -1f, 1f));

        Vector3 distanceToTarget2 = target2.position - transform.position;

        AddVectorObs(Mathf.Clamp(distanceToTarget2.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget2.z / maxRange, -1f, 1f));

        Vector3 relativePos = transform.position - pivotTransform.position;

        AddVectorObs(Mathf.Clamp(relativePos.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / maxRange, -1f, 1f));

        AddVectorObs(Mathf.Clamp(wolfRigidbody.velocity.x / maxRange, -1f, 1f));
        AddVectorObs(Mathf.Clamp(wolfRigidbody.velocity.z / maxRange, -1f, 1f));

        AddVectorObs(Mathf.Clamp(hungry / 100f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(hp / 100f, -1f, 1f));
        //AddVectorObs(state, NUM_STATE);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if (state == (int)WolfState.Normal)
        {
            //AddReward(0.005f);
            hungryPenalty = -(100f - hungry) * 0.001f;
            hpPenalty = -(100f - hp) * 0.001f;
        }
        else
        {
            hungryPenalty = -(100f - hungry) * 0.002f;
            hpPenalty = -(100f - hp) * 0.002f;
        }

        penalty = hungryPenalty + hpPenalty;
        AddReward(penalty);

        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        transform.LookAt(target);
        transform.position = transform.position + new Vector3(horizontalInput, 0f, verticalInput) * moveForce;

        if (targetEaten)
        {
            Debug.Log("먹었다!");
            ResetTarget();

            AddReward(1.0f);
            //if (state == (int)WolfState.Hungry)
            //{
            //    AddReward(1.0f);
            //}
            //else if (state == (int)WolfState.Normal)
            //{
            //    AddReward(0.5f);
            //}
            //else if (state == (int)WolfState.Died)
            //{
            //    AddReward(0.01f);
            //}

            hungry = hungry + 50f;
            hungry = Mathf.Clamp(hungry, 0f, 100f);

        }
        else if (rest)
        {
            Debug.Log("쉬고있다!");

            AddReward(0.5f);
            //if (state == (int)WolfState.Hungry)
            //{
            //    AddReward(0.01f);
            //}
            //else if (state == (int)WolfState.Normal)
            //{
            //    AddReward(0.5f);
            //}
            //else if (state == (int)WolfState.Died)
            //{
            //    AddReward(1.0f);
            //}

            hp = hp + 20f;
            hp = Mathf.Clamp(hp, 0f, 100f);
        }

        if (dead)
        {
            AddReward(-1.0f);
            Done();
        }
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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
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

        if (hungry + hp <= 150f)
        {
            if (hungry >= hp)
                state = (int)WolfState.Died;
            else
                state = (int)WolfState.Hungry;
        }
        else
        {
            state = (int)WolfState.Normal;
        }

        hungry = hungry - (Time.deltaTime * 2.0f);
        hp = hp - Time.deltaTime;

        if (hungry <= 0f)
        {
            hp = hp - Time.deltaTime;
        }

        if (hp <= 0f)
        {
            dead = true;
        }
    }

}