using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BallAgentScript : Agent
{
    private Rigidbody ballRigidBody;
    public Transform pivotTransform;    // 위치의 기준점
    public Transform target;            // 아이템 목표

    public float moveForce = 10;        // 이동 힘

    private bool targetEaten = false;   // 목표물 먹었는가?

    private bool dead = false;          // 사망 상태

    void Awake()
    {
        ballRigidBody = GetComponent<Rigidbody>();
    }

    void ResetTarget()
    {
        targetEaten = false;
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        target.position = randomPos + pivotTransform.position;
    }

    public override void AgentReset()
    {
        // agent가 reset 될 때 자동으로 실행되는 함수로, 어떻게 리셋할 지 작성하면 됨.
        // 에이전트 게임 오브젝트가 처음 활성화 되거나, 에피소드가 끝나고 다음 에피소드로 넘어가면서 학습 환경을 초기화 하거나,
        // 에이전트가 목표를 완수하거나, 목표를 실패해서 완전히 죽고 처음부터 다시 시작할 때 실행되는 함수.

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        transform.position = randomPos + pivotTransform.position;

        dead = false;
        ballRigidBody.velocity = Vector3.zero;

        ResetTarget();
    }

    public override void CollectObservations()
    {
        // 주변 관측 함수로, 에이전트가 주변을 관측하려고 할 때마다 자동으로 실행됨.
        // 에이전트 주변 사항을 측정해서 벡터 공간이라는 곳에 삽입함.
        // 에이전트가 기록된 벡터 공간의 정보를 통해서 어떤 판단을 내려야 할 지 결정함.

        // 현재 위치, 목표 아이템 사이의 거리와 방향을 측정
        Vector3 distanceTarget = target.position - transform.position;

        // y 방향으로 움직이지 않으니까 y는 기록하지 않음.
        // distanceTarget, relativePos은 -5~5 사이 값을 가지고, ballRigidBody.velocity도 -10~10 정도의 값을 가짐.
        // 정규화 되어 있어야 강화 학습 퍼포먼스가 올라감. Mathf.Clamp(..., -1f, 1f);로 이상의 값은 잘라줌.

        AddVectorObs(Mathf.Clamp(distanceTarget.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceTarget.z / 5f, -1f, 1f));

        // 발판의 중심에서 상대적으로 측정한 위치 (발판과 멀어지면 떨어지게 되므로)
        Vector3 relativePos = transform.position - pivotTransform.position;

        AddVectorObs(Mathf.Clamp(relativePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5f, -1f, 1f));

        AddVectorObs(Mathf.Clamp(ballRigidBody.velocity.x / 10f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(ballRigidBody.velocity.z / 10f, -1f, 1f));

        // Brain Parameters - Vector Observation - Space Size
        // -> AddVectorObs 개수만큼 줘야 한다. 여기선 6개로 설정했으니까 6이 된다.
        // Brains Folder 속 BallPlayerBrain에서 입력하면 된다.
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // 가만히 있으면 시간에 따라 벌점 누적
        // 주의해야 할 점: 벌점을 너무 과하게 주면 안 된다.
        // 일찍 죽는 것이 벌점을 크게 받지 않는 것이라고 인식하기 때문에.
        AddReward(-0.001f);

        // 입력통로를 2개로 가정 (3개로 해도 됨)
        // Brain Parameters - Vector Action - Space Size
        // -> 2개로 가정했으니까 2가 된다.
        // Brains Folder 속 BallPlayerBrain에서 입력하면 된다.
        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        ballRigidBody.AddForce(horizontalInput * moveForce, 0f, verticalInput * moveForce);

        // 행동에 대한 보상과 벌점
        if (targetEaten)
        {
            AddReward(1.0f);
            ResetTarget();
        }
        else if (dead)
        {
            AddReward(-1.0f);

            // Done()을 실행하면 현재까지의 결과와 보상 등의 정보를 텐서플로우에 보내고 동작을 멈춤.
            // 만약 Done()을 실행했을 때 에이전트를 리셋하는 옵션 체크되어 있다면, 
            // 자동으로 AgentReset()을 실행하고 처음부터 다시 훈련을 시작함.
            Done();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            dead = true;
        }
        else if (other.CompareTag("goal"))
        {
            targetEaten = true;
        }
    }
}
