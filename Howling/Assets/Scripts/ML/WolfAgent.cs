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
    ATTACK,
    DIG,
    GOTOPLAYER,
    ROTATION,
    MOVEORDERS,
}

public class WolfAgent : Agent
{
    [SerializeField] private Animator animator;
    [SerializeField] public GameObject digEffect;

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
    private float walkSpeed = 3f;
    [HideInInspector] public float moveForce = 100f;
    public float AttackDamage;
    public float DefendDamage;
    public float Eyesight;

    [Header("Monitoring")]
    public float Hp;
    public float Hungry;
    public float Friendly;
    public string currentAction;

    [SerializeField]
    Transform Player;
    [SerializeField]
    GameObject Players;

    /////////////////  Hide Parameters  /////////////////
    [HideInInspector] public PlayerRelation playerRelation;
    [HideInInspector] public TargetType targetType;

    [Header("Species Parameters")]
    // 랜덤 스폰하기 위한 임시적인 범위, 맵 20X20 공간에서만 가능
    private float minRange = -5f;
    private float maxRange = 5f;

    private GameObject Environment;
    private Rigidbody agentRB;
    private float nextAction;
    private bool enterDeadZone;
    private RayPerception rayPer;
    private float targetRange = 2f;

    private Vector3 direction;

    public GameObject[] dropItem;

    public bool isMoving = false;

    private void Start()
    {
        InitializeAgent();
        AgentReset();

        if (Players == null)
            Debug.Log(transform.parent.name + "웨요????");
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();

        currentAction = "Idle";
    }

    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 1f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        Hp = 100;
        Hungry = 100;
        Friendly = 0;
        walkSpeed = 3f;

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
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        if (isMoving)
        {
            transform.position += transform.forward * Time.deltaTime * walkSpeed;
        }
        DecreaseStatus();

        if (Dead)
        {
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
            if (currentAction == "Dig")
            {
                DigResult();
                return;
            }
            isMoving = false;
            currentAction = "Deciding";
            RequestDecision();
        }
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
                isMoving = true;
                animator.SetBool("isMove", true);
                MoveAgent(vectorAction);
                break;
            case (int)ActionType.EAT:
                isMoving = false;
                Eat();
                break;
            case (int)ActionType.REST:
                isMoving = false;
                Rest();
                break;
            //case (int)ActionType.GOTOPLAYER:
            //    isMoving = true;
            //    animator.SetBool("isMove", true);
            //    GoToPlayer();
            //    break;
            case (int)ActionType.ATTACK:
                isMoving = false;
                Attack();
                break;
            case (int)ActionType.DIG:
                isMoving = false;
                Dig();
                break;
        }

        if (vectorAction[(int)ActionType.GOTOPLAYER] > .5f)
        {
            isMoving = true;
            animator.SetBool("isMove", true);
            GoToPlayer();
        }
    }

    private GameObject FirstAdjacent(string tag, float range)
    {
        var colliders = Physics.OverlapSphere(transform.position, range, colliderLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag(tag))
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
        if (act[(int)ActionType.GOTOPLAYER] <= .5f)
        {
            var rotate = Mathf.Clamp(act[(int)ActionType.ROTATION], -1f, 1f);
            transform.Rotate(transform.up, rotate * 10f);
        }

        var adj = FirstAdjacent("Player", targetRange * 3);
        if (adj != null)
        {
            AddReward(0.001f);
            //Debug.Log(Mathf.Floor(Time.realtimeSinceStartup / 100) + "무브: 플레이어 근처");
            Friendly += Time.deltaTime * 2;
        }
        //else
        //    //Debug.Log(Mathf.Floor(Time.realtimeSinceStartup / 100) + "무브: 플레이어 근처 아뉨");

        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    bool CanEat
    {
        get
        {
            var adj = FirstAdjacent("item", targetRange);
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
            float oldHungy = Hungry;

            var adj = FirstAdjacent("item", targetRange);
            if (adj != null)
            {
                transform.LookAt(adj.transform);

                if (adj.GetComponent<ItemPickUP>().item.ItemName == "손질되지 않은 고기")
                {
                    Debug.Log("생고기 냠냠");
                    Hungry += 20f;
                }
                else if (adj.GetComponent<ItemPickUP>().item.ItemName == "손질된 고기")
                {
                    Debug.Log("사료 냠냠");
                    Hungry += 30f;
                    Friendly += 5f;
                }
                else if (adj.GetComponent<ItemPickUP>().item.ItemName == "사과")
                {
                    Debug.Log("사과 냠냠");
                    Hungry += 10f;
                }
                Hungry = Mathf.Clamp(Hungry, 0f, MaxHungry);

                float eatReward = (Hungry - oldHungy) * 0.03f;

                //Debug.Log("WolfAgent - 냠냠");
                AddReward(eatReward);

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
            if (FirstAdjacent("home", targetRange) != null) return true;
            return false;
        }
    }

    public void Rest()
    {
        if (CanRest)
        {
            float oldHp = Hp;

            var adj = FirstAdjacent("home", targetRange);
            if (adj != null)
            {
                float restReward = (Hp - oldHp) * (1 - (Hp / MaxHp)) * 0.03f;

                transform.LookAt(adj.transform);

                Debug.Log("rest 중!!!");

                Hp += 10f;
                Hp = Mathf.Clamp(Hp, 0f, MaxHp);

                AddReward(restReward);


                Friendly += 1f;
                //Hungry -= Time.deltaTime * 0.01f;

                nextAction = Time.timeSinceLevelLoad + (25 / RestSpeed);
                currentAction = "Resting";
            }
        }
    }

    bool CanGoToPlayer
    {
        get
        {
            float playerRange = Eyesight;
            float dist = 100;
            float maxdist = 10;

            Vector2 wolfPos = new Vector2(transform.position.x, transform.position.z);

            if (Players == null)
                Debug.Log(Players);

            for (int i = 0; i < Players.transform.childCount; i++)
            {
                Vector2 playerPos = new Vector2(Players.transform.GetChild(i).position.x, Players.transform.GetChild(i).position.z);

                if (Vector2.Distance(wolfPos, playerPos) < dist)
                {
                    dist = Vector2.Distance(wolfPos, playerPos);
                    Player = Players.transform.GetChild(i);
                }
            }

            if (maxdist <= dist && dist <= playerRange)
            {
                Debug.Log("가까움");

                walkSpeed = 5f;
                return true;
            }
            walkSpeed = 3f;
            return false;
        }
    }

    public void GoToPlayer()
    {
        if (CanGoToPlayer)
        {
            Friendly += 2f;
            Friendly = Mathf.Clamp(Friendly, 0f, MaxFriendly);

            direction = (Player.position - transform.position).normalized; // 타겟으로 향하는 방향
            direction.y = 0;

            Quaternion newRotation = Quaternion.LookRotation(direction);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 50.0f);

            var reward = 0.001f * Friendly;
            Debug.Log("Go to player reward");
            AddReward(reward);
            SetPlayerRelation();
            nextAction = Time.timeSinceLevelLoad + (25 / RestSpeed);
            currentAction = "GoToPlayer";
        }
    }

    bool CanAttack
    {
        get
        {
            var testvic = FirstAdjacent("enemyCollider", targetRange);

            if (testvic != null)
            {
                //Debug.Log("enemy 발견!");
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
            vic = FirstAdjacent("enemyCollider", targetRange).GetComponentInParent<Enemy>();

            transform.LookAt(vic.transform);

            if (vic != null)
            {
                vic.DecreaseHpByWolf((int)AttackDamage, this);
            }
            //else Debug.Log("WolfAgent - Attack, vic is null.");
        }
        //Hungry -= Time.deltaTime * 0.01f; // 공격했으니 허기소비
    }

    public void EnemyAtkReward(int atk)
    {
        //Debug.Log("WolfAgent - EnemyAtkReward, 공격했다!");
        AddReward(atk * 0.05f);
    }

    public void EnemyDieReward()
    {
        //Debug.Log("WolfAgent - EnemyDieReward, 해치웠다!");
        AddReward(.3f);
    }

    bool CanDig
    {
        get
        {
            var colliders = Physics.OverlapSphere(transform.position, 20f);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("item") ||
                   collider.CompareTag("home") ||
                   collider.CompareTag("enemyCollider") ||
                   collider.CompareTag("Player"))
                {
                    //Debug.Log("앞에 뭐가 있다!!!");
                    return false;
                }
            }
            //Debug.Log("앞에 암것도 없으니 땅을 파겠다!!");
            return true;

        }
    }

    void Dig()
    {
        if (CanDig)
        {
            OnDigEffect();
            currentAction = "Dig";

            digEffect.SetActive(true);

            //Hungry -= Time.deltaTime * 1f;

            animator.SetBool("isDig", true);

            nextAction = Time.timeSinceLevelLoad + (25 / DigSpeed);
        }
    }

    void DigResult()
    {
        if (Random.Range(0.0f, 1.0f) <= 0.3f) // 땅파기 성공!
        {
            currentAction = "DigSuccess";

            DropItem();

            AddReward(.01f); // 성공 보상
            Debug.Log("땅파기 성공!!");
        }
        else
        {
            Debug.Log("땅파기 실패!!");

            currentAction = "DigFail";
        }

        animator.SetBool("isDig", false);

        digEffect.SetActive(false);
    }

    IEnumerator OnDigEffect()
    {
        digEffect.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        //digEffect.SetActive(false);
    }

    void DropItem()
    {
        //Debug.Log("WolfAgent - 땅파기 성공!!");
        int itemIndex = Random.Range(0, dropItem.Length);

        // 땅파서 아이템 나오는 위치 보고,, 수정하던지 해야할듯,,!
        Instantiate(dropItem[itemIndex], transform.position, Quaternion.identity);
    }

    bool Dead
    {
        get
        {
            if (Hp <= 0 || enterDeadZone || Hungry <= 0)
            {
                //if (Hp <= 0) Debug.Log("hp <= 0");
                //else if (enterDeadZone) Debug.Log("enterDeadZone");
                //else if (Hungry <= 0) Debug.Log("Hungry <= 0");
                return true;
            }

            return false;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("dead"))
        {
            enterDeadZone = true;
        }
    }

    public void SetPlayerRelation()
    {
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