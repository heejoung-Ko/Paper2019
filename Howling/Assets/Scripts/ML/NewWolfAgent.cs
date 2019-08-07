using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public enum PlayerRelation
{
    Enemy, Stranger, Friend, Soulmate
}

public enum TargetType
{
    Player,
    Home,
    Foods,
    Enemies
}

public enum ActionType
{
    MOVE,
    EAT,
    REST,
    GOTOPLAYER,
    ATTACK,
    DEFEND,
    MOVEORDERS,
    ROTATION,
}

public class NewWolfAgent : Agent
{
    [Header("Creature Parameters")]
    public Transform pivotTransform; // 임시 위치 기준점, 트레이닝룸 위치
    public Transform targetPlayer;
    public Transform targetHome;
    //public Transform targetFood;
    //public Transform targetEnemy;
    [SerializeField] private Transform target;

    [Header("Creature Points (100 Max)")]
    public float MaxHp;
    public float MaxHungry;
    public float MaxFriendly;
    public float EatingSpeed;
    public float MaxSpeed;
    public float AttackDamage;
    public float DefendDamage;
    public float Eyesight;

    public float moveForce;

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

    // Penalty
    //private float penalty = 0f;
    //private float hpPenalty = 0f;
    //private float hungryPenalty = 0f;
    //private float friendlyPenalty = 0f;

    private GameObject Environment;
    private Rigidbody agentRB;
    private RayPerception rayPer;
    private float nextAction;

    private bool died;
    private bool enterDeadZone;
    private bool canRest;
    private float canRestTime;

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        InitializeAgent();
        AgentReset();
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
        //penalty = 0f;
        //hpPenalty = 0f;
        //hungryPenalty = 0f;
        //friendlyPenalty = 0f;
        died = false;
        enterDeadZone = false;
        target = null;
    }

    public override void AgentOnDone()
    {
    }

    private float BoolToFloat(bool val)
    {
        if (val) return 1.0f;
        else return 0.0f;
    }

    public override void CollectObservations()
    {
        float rayDistance = Eyesight;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "food", "home", "herbivore", "carnivore" };
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
        //Action Space 6 float (Ctl+G line 19)

        int maxAction = 0;
        for (int i = 0; i < (int)ActionType.ATTACK; ++i)
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
                MoveAgent(vectorAction);
                break;
            case (int)ActionType.EAT:
                Eat();
                break;
            case (int)ActionType.REST:
                Rest();
                break;
            case (int)ActionType.GOTOPLAYER:
                GoToPlayer();
                break;
            case (int)ActionType.ATTACK:
                Attack();
                break;
        }

        AddPenalty();

        //if (vectorAction[(int)ActionType.MOVE] > .5)
        //{
        //    MoveAgent(vectorAction);
        //}
        //else if (vectorAction[(int)ActionType.EAT] > .5)
        //{
        //    Eat();
        //}
        //else if (vectorAction[(int)ActionType.REST] > .5)
        //{
        //    Rest();
        //}
        //else if (vectorAction[(int)ActionType.GOTOPLAYER] > .5)
        //{
        //    GoToPlayer();
        //}
        //else if (vectorAction[(int)ActionType.ATTACK] > .5)
        //{
        //    Attack();
        //}
        //Defend();
    }

    void Defend()
    {
        currentAction = "Defend";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    void Attack()
    {
        currentAction = "Attack";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
        var testvic = FirstAdjacent("herbivore");
        if (testvic == null)
        {
            testvic = FirstAdjacent("carnivore");
            if (testvic == null) { target = null; return; }
        }
        var vic = FirstAdjacent("herbivore").GetComponent<MLTestEnemy>();
        if (vic != null)
        {
            Debug.Log("herbivore enemy!");
        }
        else
        {
            vic = FirstAdjacent("carnivore").GetComponent<MLTestEnemy>();
            if (vic != null)
            {
                Debug.Log("carnivore enemy!");
            }
            else { target = null; return; }
        }

        if (vic != null)
        {
            target = vic.transform;
            vic.Hp -= AttackDamage;
            Hp -= vic.AttackDamage;
            if (vic.Hp <= 0)
            {
                AddReward(.25f);
                target = null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            enterDeadZone = true;
        }
    }

    void Update()
    {
        if (Dead) return;
        if (canRest) canRestTime += 1;
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
        if (Hungry <= 0)
        {
            Hungry = 0f;
            Hp -= Time.deltaTime * 0.5f;
            if (Hp <= 0f) Hp = 0f;
        }
    }

    public void AddPenalty()
    {
        float hpPenalty = Hp / MaxHp * 0.001f;
        float hungryPenalty = Hungry / MaxHungry * 0.001f;
        AddReward(hpPenalty + hungryPenalty);
    }

    public void MonitorLog()
    {
        Monitor.Log("Action", currentAction, transform);
        Monitor.Log("Hp", Hp / MaxHp, transform);
        Monitor.Log("Hungry", Hungry / MaxHungry, transform);
        Monitor.Log("Friendly", Friendly / MaxFriendly, transform);
    }

    bool CanEat
    {
        get
        {
            if (FirstAdjacent("food") != null) return true;
            return false;
        }
    }

    bool CanRest
    {
        get
        {
            if (FirstAdjacent("home") != null)
            {
                canRest = true;
                if (canRestTime > 200)
                {
                    canRestTime = 0;
                    return true;
                }
            }
            else canRest = false;
            return false;
        }
    }

    bool CanGoToPlayer
    {
        get
        {
            // TODO: playerRelation 따라서 판단. (일단 임시로 정했음.)
            if (playerRelation >= PlayerRelation.Friend) return true;
            return false;
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

    public void SetPlayerRelation() // TODO: Friendly 값 바뀌는 곳에 이 함수 추가하기
    {
        // TODO: playerRelation 상태 바뀌는 기준 정하기. (일단 임시로 정했음.)
        if (Friendly < MaxFriendly * 0.2) playerRelation = PlayerRelation.Enemy;
        else if (Friendly < MaxFriendly * 0.5) playerRelation = PlayerRelation.Stranger;
        else if (Friendly < MaxFriendly * 0.8) playerRelation = PlayerRelation.Friend;
        else playerRelation = PlayerRelation.Soulmate;
    }

    bool Dead
    {
        get
        {
            if (died) return true;

            if (Hp <= 0 || enterDeadZone)
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

    public void Eat()
    {
        if (CanEat)
        {
            var adj = FirstAdjacent("food");
            if (adj != null)
            {
                target = adj.transform;

                Destroy(adj);
                Hungry += 20f;
                Hungry = Mathf.Clamp(Hungry, 0f, 100f);

                AddReward(.5f);
                nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                currentAction = "Eating";
            }
            else target = null;
        }
        else target = null;
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
                    target = targetHome;
                    AddReward(.25f);
                }

                Hp += 20f;
                Hp = Mathf.Clamp(Hp, 0f, 100f);

                nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
                currentAction = "Resting";
            }
            else target = null;
        }
        else target = null;
    }

    public void GoToPlayer()
    {
        // TODO: tag == "Player" 조건 추가
        if (CanGoToPlayer)
        {
            target = targetPlayer;
        }
        else
        {
            target = null;
        }
    }

    public void MoveAgent(float[] act)
    {
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

        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }
}