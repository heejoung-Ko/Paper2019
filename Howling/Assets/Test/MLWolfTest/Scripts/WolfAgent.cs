using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Wolf : Agent
{
    public enum WolfState { idle, walk, trace, eat }
    public WolfState state = WolfState.idle;


    public float lookRadius = 10f;

    // 스테이터스 - 체력, 허기, 친밀도
    public float hp = 100f;
    public float hunger = 0f;
    public float friendship = 0f;

    // 무게
    public float mass = 45f;

    // 속도, 가속도
    public float velocity = 0.0f;
    public float walkAcc = 0.7f;
    public float runAcc = 1.5f;


    private Rigidbody wolfRigidBody;

    public Transform pivotTransform; // 위치의 기준점
    public Transform target;

    public float moveForce = 10f;
    private bool targetEaten = false; // 목표를 먹었는가

    private bool dead = false; // 사망했는가

    private void Awake()
    {
        wolfRigidBody = GetComponent<Rigidbody>();
    }

    void ResetTarget()
    {
        targetEaten = false;
//        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
//        target.position = randomPos + pivotTransform.position;
    }


    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(5f, -5f));
        transform.position = randomPos + pivotTransform.position;

        dead = false;
        wolfRigidBody.velocity = Vector3.zero;

        ResetTarget();
    }

    // Agent 주변 상황을 측정해서 Actor의 행동을 결정하게 한다
    // Agent의 눈과 귀를 만드는거
    public override void CollectObservations()
    {
        Vector3 distanceToTarget = target.position - transform.position;

        // -5 ~ +5
        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5f, -1f, 1f));

        Vector3 relativePos = transform.position - pivotTransform.position;

        // -5 ~ +5
        AddVectorObs(Mathf.Clamp(relativePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5f, -1f, 1f));

        // -10 ~ +10 -> -1 ~ +1
        AddVectorObs(Mathf.Clamp(wolfRigidBody.velocity.x / 10f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(wolfRigidBody.velocity.z / 10f, -1f, 1f));

        // 굳이 y 정보는 필요없다 -> 퍼포먼스 향상을 위해 필요없는건 생략하자~
        // + 위에 들어갈 값의 범위를 -1 ~ +1 사이로 압축 = 정규화 하는것도 퍼포먼스 향상을 위한 일!

        // AddVectorObs를 6번했으니 brain파라미터에서 SpaceSize를 6으로 조정해줘야함
    }


    // Agent가 어떤 행동을 취할지, 어떤 행동을 취했을 때 얻을 Reword 정해주기
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // 가만히 있을 때의 벌점! (벌점이 너무 강하면 이걸 피하기 위해 Agent가 죽어버린다...!)
        // 선택을 실행하는 주기가 보통 0.02초이다. 0.02초마다 0.001f의 벌점을 받는것!
        AddReward(-0.001f);
        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        wolfRigidBody.AddForce(horizontalInput * moveForce, 0f, verticalInput * moveForce);

        if(hunger > 50f)
        {
            if (targetEaten)
            {
                AddReward(0.5f);
                ResetTarget();
            }
            else if (dead)
            {
                AddReward(-1f);
                Done();
            }
        } else if (hunger <= 50f)
        {
            AddReward(-0.002f);
            if(targetEaten)
            {
                AddReward(1f);
                ResetTarget();
            } else if (dead)
            {
                AddReward(-1f);
                Done();
            }
        }                                                                                                                               
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
