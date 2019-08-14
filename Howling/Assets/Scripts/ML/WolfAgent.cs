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
    DIG,
    ROTATION,
    MOVEORDERS,
}

public class WolfAgent : Agent
{
    [SerializeField]
    private Animator animator;

    [Header("Creature Parameters")]
    public Transform pivotTransform; // 임시 위치 기준점, 트레이닝룸 위치

    public LayerMask colliderLayerMask;

    //public Transform targetFood;
    //public Transform targetEnemy;

    [Header("Creature Points (100 Max)")]
    public float MaxHp;
    public float MaxHungry;
    public float MaxFriendly;
    public float EatingSpeed;
    public float RestSpeed;
    public float DigSpeed;
    public float MaxSpeed;
    public float AttackDamage;
    public float DefendDamage;
    public float Eyesight;

    [Header("Monitoring")]
    public float Hp;
    public float Hungry;
    public float Friendly;
    public string currentAction;

    /////////////////  Hide Parameters  /////////////////
    [HideInInspector] public PlayerRelation playerRelation;
    [HideInInspector] public TargetType targetType;

    [Header("Species Parameters")]
    // 랜덤 스폰하기 위한 임시적인 범위, 맵 20X20 공간에서만 가능
    private float minRange = -40f;
    private float maxRange = 40f;

    private GameObject Environment;
    private Rigidbody agentRB;
    private float nextAction;
    private bool enterDeadZone;
    private RayPerception rayPer;

    public GameObject[] dropItem;

    //public Vector3 GetPos()
    //{
    //    return transform.position;
    //}
    //public void SetPos(Vector3 savePos)
    //{
    //    transform.position = savePos;
    //}

    //public Vector3 GetRot()
    //{
    //    return transform.eulerAngles;
    //}
    //public void SetRot(Vector3 saveRot)
    //{
    //    transform.eulerAngles = saveRot;
    //}

    //public float GetHp()
    //{
    //    return Hp;
    //}
    //public void SetHp(float saveHp)
    //{
    //    Hp = saveHp;
    //}

    //public float GetHungry()
    //{
    //    return Hungry;
    //}
    //public void SetHungry(float saveHungry)
    //{
    //    Hungry = saveHungry;
    //}

    //public float GetFriendly()
    //{
    //    return Friendly;
    //}
    //public void SetFriendly(float saveFriendly)
    //{
    //    Friendly = saveFriendly;
    //}

    private void Start()
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
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 20f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        Hp = 100;
        Hungry = 100;
        Friendly = 0;

        currentAction = "Idle";
        playerRelation = PlayerRelation.Enemy;
        enterDeadZone = false;
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
        if (Dead)
        {
            animator.SetTrigger("deadTrigger");

            currentAction = "Dead";
            AddReward(-1f);
            Done();
            AgentReset();
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
        Hungry -= Time.deltaTime * 1f;
    }

    public override void AgentOnDone()
    {
    }

    public override void CollectObservations()
    {
        float rayDistance = Eyesight;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "item", "home", "enemyCollider", "Player" };
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
        AddVectorObs(BoolToFloat(CanAttack));
        AddVectorObs(BoolToFloat(CanDig));

        // TODO: 플레이어와의 거리 or 플레이어 위치 추가? (일단 없앴음.) 
        //AddVectorObs(targetPlayer.position);
        //AddVectorObs((int)playerRelation);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Action Space 8 float

        int maxAction = 0;
        for (int i = 0; i < (int)ActionType.DIG; ++i)
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
                Eat();
                break;
            case (int)ActionType.REST:
                Rest();
                break;
            case (int)ActionType.GOTOPLAYER:
                animator.SetBool("isWalk", true);
                GoToPlayer();
                animator.SetBool("isWalk", false);
                break;
            case (int)ActionType.ATTACK:
                Attack();
                break;
            case (int)ActionType.DIG:
                Dig();
                break;
        }
    }

    private GameObject FirstAdjacent(string tag)
    {
        var colliders = Physics.OverlapSphere(transform.position, 2f, colliderLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == tag)
            {
                if (collider.CompareTag("item"))
                {
                    var adj = collider.gameObject;

                    if (adj.GetComponent<ItemPickUP>().item.ItemName == "손질되지 않은 고기" ||
                        adj.GetComponent<ItemPickUP>().item.ItemName == "손질된 고기" ||
                        adj.GetComponent<ItemPickUP>().item.ItemName == "사과")
                    {
                        return collider.gameObject;
                    }
                    else return null;
                }
                return collider.gameObject;
            }
        }
        return null;
    }


    // Actions
    public void MoveAgent(float[] act)
    {
        var rotate = Mathf.Clamp(act[(int)ActionType.ROTATION], -1f, 1f);
        transform.Rotate(transform.up, rotate * 25f);

        //if (act[(int)ActionType.MOVEORDERS] > .5f)

        //{
            transform.position += transform.forward;
            //AddReward(0.0001f);
        //}

        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    bool CanEat
    {
        get
        {
            var adj = FirstAdjacent("item");
            if (adj != null)
            {
                return true;
            }
            return false;
        }
    }

    public void Eat()
    {
        if (CanEat)
        {
            animator.SetTrigger("eatTrigger");

            var adj = FirstAdjacent("item");
            if (adj != null)
            {
                transform.LookAt(adj.transform);

                if (adj.GetComponent<ItemPickUP>().item.ItemName == "손질되지 않은 고기")
                {
                    Debug.Log("생고기 냠냠");
                    Hungry += 10f;
                    AddReward(0.1f);
                }

                if (adj.GetComponent<ItemPickUP>().item.ItemName == "손질된 고기")
                {
                    Debug.Log("사료 냠냠");
                    Hungry += 20f;
                    Friendly += 5f;
                    AddReward(0.25f);
                }

                if (adj.GetComponent<ItemPickUP>().item.ItemName == "사과")
                {
                    Debug.Log("사과 냠냠");
                    Hungry += 3f;
                    AddReward(0.05f);
                }

                Hungry = Mathf.Clamp(Hungry, 0f, MaxHungry);
                Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

                Destroy(adj);
                SetPlayerRelation();
                nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                currentAction = "Eating";
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
            animator.SetTrigger("restTrigger");

            var adj = FirstAdjacent("home");
            if (adj != null)
            {
                transform.LookAt(adj.transform);

                Debug.Log("rest 중!!!");

                if (Hp < 90)
                {
                    AddReward(.01f);
                }

                Hp += 10f;
                Hp = Mathf.Clamp(Hp, 0f, MaxHp);

                Friendly += 1f;
                Hungry -= Time.deltaTime * 0.01f;

                nextAction = Time.timeSinceLevelLoad + (25 / RestSpeed);
                currentAction = "Resting";
            }
        }
    }

    bool CanGoToPlayer
    {
        get
        {
            if (FirstAdjacent("Player") != null) return true;
            return false;

            //// TODO: playerRelation 따라서 판단. (일단 임시로 정했음.)
            //if (playerRelation >= PlayerRelation.Stranger)
            //{
            //    if (FirstAdjacent("Player") != null) return true;
            //    return false;
            //}
        }
    }

    public void GoToPlayer()
    {
        if (CanGoToPlayer)
        {
            Friendly += 5f;
            Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

            AddReward(0.001f * Friendly);
            SetPlayerRelation();
            nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
            currentAction = "GoToPlayer";
        }
    }

    bool CanAttack
    {
        get
        {
            var testvic = FirstAdjacent("enemyCollider");

            if (testvic != null)
            {
                Debug.Log("enemy 발견!");
                return true;
            }
            else
            {
                //Debug.Log("공격대상이 없당");
                return false;
            }
        }
    }

    void Attack()
    {
        if (CanAttack)
        {
            currentAction = "Attack";
            nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
            Enemy vic = null;

            //Debug.Log("공격!");
            vic = FirstAdjacent("enemyCollider").GetComponentInParent<Enemy>();
            Vector3 lookVector = vic.transform.position;
            lookVector.y = transform.position.y;
            transform.LookAt(lookVector);

            if (vic != null)
            {
                animator.SetTrigger("attackTrigger");
                vic.DecreaseHpByWolf((int)AttackDamage);
            }
            else Debug.Log("WolfAgent - Attack, vic is null.");
        }
        Hungry -= Time.deltaTime * 0.01f; // 공격했으니 허기소비
    }

    public void EnemyAtkReward(int atk)
    {
        Debug.Log("WolfAgent - EnemyAtkReward, 공격했다!");
        AddReward(atk * 0.002f);
    }

    public void EnemyDieReward()
    {
        Debug.Log("WolfAgent - EnemyDieReward, 해치웠다!");
        AddReward(.1f);
    }

    bool CanDig
    {
        get
        {
            // TODO: 가까이에 타겟 없을 때 return true;
            return false;
        }
    }

    void Dig()
    {
        if (CanDig)
        {
            animator.SetTrigger("digTrigger");

            currentAction = "Dig";

            Hungry -= Time.deltaTime * 1f;

            if (Random.Range(0.0f, 1.0f) <= 0.3f) // 땅파기 성공!
            {
                animator.SetTrigger("successTrigger");
                currentAction = "DigSuccess";

                DropItem();

                AddReward(.05f); // 성공 보상
            }
            else
            {
                Debug.Log("땅파기 실패!!");

                animator.SetTrigger("failTrigger");
                currentAction = "DigFail";
            }

            nextAction = Time.timeSinceLevelLoad + (25 / DigSpeed);

        }
    }

    void DropItem()
    {
        Debug.Log("땅파기 성공!!");
        int itemIndex = Random.Range(0, dropItem.Length);

        // 땅파서 아이템 나오는 위치 보고,, 수정하던지 해야할듯,,!
        //Instantiate(dropItem[itemIndex], transform.position, Quaternion.identity);
    }

    bool Dead
    {
        get
        {
            if (Hp <= 0 || enterDeadZone || Hungry <= 0)
            {
                if (Hp <= 0) Debug.Log("hp <= 0");
                else if (enterDeadZone) Debug.Log("enterDeadZone");
                else if (Hungry <= 0) Debug.Log("Hungry <= 0");
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