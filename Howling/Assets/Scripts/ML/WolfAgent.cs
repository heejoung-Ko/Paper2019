using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WolfAgent : Agent
{
    private Animator animator;

    [Header("Creature Parameters")]
    public Transform pivotTransform; // 임시 위치 기준점, 트레이닝룸 위치
    public Transform targetPlayer;
    public Transform targetHome;
    //public Transform targetFood;
    //public Transform targetEnemy;
    //[SerializeField] private Transform target;

    [Header("Creature Points (100 Max)")]
    public float MaxHp;
    public float MaxHungry;
    public float MaxFriendly;
    public float EatingSpeed;
    public float RestSpeed;
    public float MaxSpeed;
    public float AttackDamage;
    public float DefendDamage;
    public float Eyesight;

    [Header("Monitoring")]
    public float Hp;
    public float Hungry;
    public float Friendly;
    public string currentAction;

    [Header("Species Parameters")]


    /////////////////  Hide Parameters  /////////////////
    [HideInInspector] public PlayerRelation playerRelation;
    [HideInInspector] public TargetType targetType;


    // 랜덤 스폰하기 위한 임시적인 범위, 맵 20X20 공간에서만 가능
    private float minRange = -10f;
    private float maxRange = 10f;

    private GameObject Environment;
    private Rigidbody agentRB;
    private float nextAction;
    private bool died;
    private bool enterDeadZone;
    private RayPerception rayPer;

    private void Awake()
    {
        InitializeAgent();
        AgentReset();
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();

        currentAction = "Idle";
    }

    public override void AgentReset()   // TODO: Reset 할 곳에 추가하기
    {
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 1f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;

        Hp = 100;
        Hungry = 100;
        Friendly = 0;

        currentAction = "Idle";
        playerRelation = PlayerRelation.Enemy;
        died = false;
        //enterDeadZone = false;
        //target = null;
    }

    public void MonitorLog()
    {
        Monitor.Log("Action", currentAction, transform);
        Monitor.Log("Hp", Hp / MaxHp, transform);
        Monitor.Log("Hungry", Hungry / MaxHungry, transform);
        Monitor.Log("Friendly", Friendly / MaxFriendly, transform);
    }

    void Update()
    {
        if (Dead) return;
        MonitorLog();
    }

    public void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > nextAction)
        {
            currentAction = "Deciding";
            RequestDecision();
        }

        DecreaseStatus();
    }

    public void DecreaseStatus()
    {
        Hungry -= Time.deltaTime * 0.5f;
    }

    public override void AgentOnDone()
    {
    }

    public override void CollectObservations()
    {
        float rayDistance = Eyesight;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "food", "home", "feed", "player" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f)); // rayAngles * (detectableObjects + 2) = 42
        Vector3 localVelocity = transform.InverseTransformDirection(agentRB.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(Hp);
        AddVectorObs(Hungry);
        AddVectorObs(Friendly);
        AddVectorObs(BoolToFloat(CanEat));
        AddVectorObs(BoolToFloat(CanRest));
        AddVectorObs(BoolToFloat(CanGoToPlayer));

        // TODO: 플레이어와의 거리 or 플레이어 위치 추가? (일단 없앴음.) 
        //AddVectorObs(targetPlayer.position);
        //AddVectorObs((int)playerRelation);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Action Space 6 float

        int maxAction = 0;
        for (int i = 0; i < (int)ActionType.DEFEND; ++i)
        {
            if (vectorAction[i] < vectorAction[i + 1])
            {
                if (vectorAction[i + 1] > .5f)
                    maxAction = i + 1;
            }
        }

        switch (maxAction)
        {
            case (int)ActionType.MOVE:
                animator.SetBool("isWalk", true);
                MoveAgent(vectorAction);
                animator.SetBool("isWalk", false);
                break;
            case (int)ActionType.EAT:
                animator.SetTrigger("eatTrigger");
                Eat();
                break;
            case (int)ActionType.REST:
                animator.SetTrigger("restTrigger");
                Rest();
                break;
            case (int)ActionType.GOTOPLAYER:
                animator.SetBool("isWalk", true);
                GoToPlayer();
                animator.SetBool("isWalk", false);
                break;
            case (int)ActionType.ATTACK:
                animator.SetTrigger("attackTrigger");
                Attack();
                break;
            case (int)ActionType.DEFEND:
                animator.SetTrigger("hitTrigger");
                Defend();
                break;
        }
    }

    private GameObject FirstAdjacent(string tag)
    {
        var colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == tag)
            {
                return collider.gameObject;
            }
        }
        return null;
    }
    

    // Actions
    public void MoveAgent(float[] act)
    {
        Vector3 rotateDir = Vector3.zero;
        rotateDir = transform.up * Mathf.Clamp(act[(int)ActionType.ROTATION], -1f, -1f);

        if (act[(int)ActionType.MOVEORDERS] > .5f)
        {
            transform.position += transform.forward;
        }

        Hp -= .01f;
        transform.Rotate(rotateDir, Time.fixedDeltaTime * MaxSpeed);
        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);


        /*
        float d1 = 0, d2 = 0;

        if (target != null)
        {
            transform.LookAt(target);
            d1 = (target.position - transform.position).sqrMagnitude;
        }
        else
        {
            var rotate = Mathf.Clamp(act[(int)ActionType.ROTATION], -1f, 1f);
            transform.Rotate(transform.up, rotate * 25f);
        }

        if (act[(int)ActionType.MOVEORDERS] > .5f)
        {
            transform.position += transform.forward;
        }


        if (target != null)
        {
            d2 = (target.position - transform.position).sqrMagnitude;
            if (d1 > d2)
            {
                float distanceReward = .001f;
                if (distanceReward >= 0) AddReward(distanceReward);
            }
            else
            {
                float distancePenalty = -.1f;
                AddReward(distancePenalty);
            }
        }
        */
    }

    bool CanEat
    {
        get
        {
            if (FirstAdjacent("food") != null || FirstAdjacent("feed") != null) return true;
            return false;
        }
    }

    public void Eat()
    {
        if (CanEat)
        {
            var adj = FirstAdjacent("food");
            if (adj != null)
            {
                Destroy(adj);
                Hungry += 10f;
                Hungry = Mathf.Clamp(Hungry, 0f, MaxHungry);

                AddReward(.01f);
                nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                currentAction = "Eating";
            } else
            {
                adj = FirstAdjacent("feed");
                if(adj != null)
                {
                    Destroy(adj);

                    Hungry += 10f;
                    Hungry = Mathf.Clamp(Hungry, 0f, MaxHungry);

                    Friendly += 5f;
                    Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);
                    AddReward(.02f);
                    nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                    currentAction = "Eating";
                }
            }
        }
    }

    bool CanRest
    {
        get
        {
            if (FirstAdjacent("home") != null) return true;
            return false;
        }
    }

    public void Rest()
    {
        if (CanRest)
        {
            var adj = FirstAdjacent("home");
            if (adj != null)
            {
                Debug.Log("rest 중!!!");

                if (Hp < 100)
                {
                    AddReward(.01f);
                }

                Hp += 10f;
                Hp = Mathf.Clamp(Hp, 0f, MaxHp);

                nextAction = Time.timeSinceLevelLoad + (25 / RestSpeed);
                currentAction = "Resting";
            }
        }
    }

    bool CanGoToPlayer
    {
        get
        {
            // TODO: playerRelation 따라서 판단. (일단 임시로 정했음.)
            if (playerRelation >= PlayerRelation.Friend)
            {
                if (FirstAdjacent("Player") != null) return true;
                return false;
            }
            return false;
        }
    }

    public void GoToPlayer()
    {
        // TODO: tag == "Player" 조건 추가
        if (CanGoToPlayer)
        {
            Friendly += 5f;
            Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

            AddReward(.01f);
            nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
            currentAction = "GoToPlayer";
        }
    }

    void Attack()
    {
        float damage = 0f;

        currentAction = "Attack";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);

        var target = FirstAdjacent("enemy").GetComponent<Enemy>();
        
        if(target != null)
        {
            damage = AttackDamage;
        }
        else
        {
            Debug.Log("공격할 대상이 없당");
        }

        if(damage > 0)
        {
            target.DecreaseHp((int)damage);
            AddReward(.01f); // 공격 보상
            if(target.state == Enemy.EnemyState.die)
            {
                AddReward(.25f); // 사냥 성공 보상
            }
        }

        Hungry -= .1f;
    }

    void Defend()
    {
        currentAction = "Defend";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    bool Dead
    {
        get
        {
            if (died) return true;

            if (Hp <= 0 || enterDeadZone || Hungry <= 0)
            {
                currentAction = "Dead";
                died = true;
                AddReward(-1f);
                Done();
                AgentReset();
                return true;
            }

            return false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            enterDeadZone = true;
        }
    }

    public void SetPlayerRelation() // TODO: Friendly 값 바뀌는 곳에 이 함수 추가하기
    {
        // TODO: playerRelation 상태 바뀌는 기준 정하기. (일단 임시로 정했음.)
        if (Friendly < MaxFriendly * 0.2) playerRelation = PlayerRelation.Enemy;
        else if (Friendly < MaxFriendly * 0.5) playerRelation = PlayerRelation.Stranger;
        else if (Friendly < MaxFriendly * 0.8) playerRelation = PlayerRelation.Friend;
        else playerRelation = PlayerRelation.Soulmate;
    }

    private float BoolToFloat(bool val)
    {
        if (val) return 1.0f;
        else return 0.0f;
    }
}