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
    //[SerializeField] private Transform target;
    public LayerMask colliderLayerMask;

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

    private GameObject Environment;
    private Rigidbody agentRB;
    private RayPerception rayPer;
    private float nextAction;

    private bool died;
    private bool enterDeadZone;
    private bool canRest;
    [SerializeField] private float canRestTime = 0;

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InitializeAgent();
        AgentReset();
    }

    public override void AgentReset()   // TODO: Reset 할 곳에 추가하기
    {
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 1f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Hp = 100;
        Hungry = 100;
        Friendly = 0;
        currentAction = "Idle";
        playerRelation = PlayerRelation.Enemy;
        died = false;
        enterDeadZone = false;
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
        string[] detectableObjects = { "item", "home", "enemy", "Player" };
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

        //AddPenalty();

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
        Enemy vic = null;
        var testvic = FirstAdjacent("herbivore");
        if (testvic != null)
        {
            vic = FirstAdjacent("herbivore").GetComponent<Enemy>();
            if (vic == null) return;
            Debug.Log("herbivore enemy!");
        }
        else if (testvic == null)
        {
            testvic = FirstAdjacent("carnivore");
            if (testvic != null)
            {
                vic = FirstAdjacent("carnivore").GetComponent<Enemy>();
                if (vic == null) return;
                Debug.Log("carnivore enemy!");
            }
            else if (testvic == null) return;
        }

        if (vic != null)
        {
            //vic.Hp -= AttackDamage;
            //Hp -= vic.AttackDamage;
            AddReward(.01f);
            //if (vic.Hp <= 0) AddReward(.25f);
            if (Hp < 0) Hp = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("home"))
        {
            canRest = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("home"))
        {
            if (canRestTime > 20)
                canRest = false;
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
        if (canRest)
        {
            canRestTime += 1;
            if (canRestTime > 20)
            {
                canRest = false;
                canRestTime = 0;
            }
        }
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
        //if (Hungry <= 0)
        //{
        //    Hungry = 0f;
        //    Hp -= Time.deltaTime * 0.01f;
        //    if (Hp <= 0f) Hp = 0f;
        //}
    }

    //public void AddPenalty()
    //{
    //    float hpPenalty = Hp / MaxHp * 0.01f;
    //    float hungryPenalty = Hungry / MaxHungry * 0.01f;
    //    AddReward(hpPenalty + hungryPenalty);
    //}

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
            if (FirstAdjacent("item") != null) return true;
            return false;
        }
    }

    bool CanRest
    {
        get
        {
            if (FirstAdjacent("home") != null)
            {
                if (canRestTime == 0) return true;
            }
            return false;
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

    private GameObject FirstAdjacent(string tag)
    {
        var colliders = Physics.OverlapSphere(transform.position, 3f, colliderLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("item"))
            {
                var adj = collider.gameObject;
                if (adj.GetComponent<ItemPickUP>().item.ItemName == "Meat" ||
                    adj.GetComponent<ItemPickUP>().item.ItemName == "Fillet")
                {
                    return collider.gameObject;
                }
                else return null;
            }

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

    public void Eat()
    {
        if (CanEat)
        {
            var adj = FirstAdjacent("item");
            if (adj != null)
            {
                if (adj.GetComponent<ItemPickUP>().item.ItemName == "Meat")
                {
                    Hungry += 5f;
                    AddReward(0.05f);
                }

                if (adj.GetComponent<ItemPickUP>().item.ItemName == "Fillet")
                {
                        Hungry += 10f;
                    Friendly += 5f;
                    AddReward(0.1f);
                }

                Hungry = Mathf.Clamp(Hungry, 0f, MaxHungry);
                Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

                Destroy(adj);
                nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                currentAction = "Eating";
            }
        }
    }

    public void Rest()
    {
        if (canRestTime == 0 && canRest)
        {
            var adj = FirstAdjacent("home");
            if (adj != null)
            {
                Debug.Log("rest 중!!!");

                if (Hp < 100)
                {
                    AddReward(.01f);
                }

                Hp += 20f;
                Hp = Mathf.Clamp(Hp, 0f, 100f);

                nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
                currentAction = "Resting";
            }
        }
    }

    public void GoToPlayer()
    {
        if (CanGoToPlayer)
        {
            Friendly += 5f;
            Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

            AddReward(.01f);
            nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
            currentAction = "GoToPlayer";
        }
    }

    public void MoveAgent(float[] act)
    {
        //float d1 = 0, d2 = 0;

        //if (target != null)
        //{
        //    transform.LookAt(target);
        //    //d1 = (target.position - transform.position).sqrMagnitude;
        //}
        //else
        //{
            var rotate = Mathf.Clamp(act[(int)ActionType.ROTATION], -1f, 1f);
            transform.Rotate(transform.up, rotate * 25f);
        //}

        if (act[(int)ActionType.MOVEORDERS] > .5f)
        {
            transform.position += transform.forward * moveForce;
        }

        //if (target != null)
        //{
        //    d2 = (target.position - transform.position).sqrMagnitude;
        //    if (d1 > d2)
        //    {
        //        float distanceReward = .001f;
        //        if (distanceReward >= 0) AddReward(distanceReward);
        //    }
        //    else
        //    {
        //        float distancePenalty = -.1f;
        //        AddReward(distancePenalty);
        //    }
        //}

        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }
}