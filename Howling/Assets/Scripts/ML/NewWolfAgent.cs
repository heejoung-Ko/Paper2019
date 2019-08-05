using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public enum PlayerRelation
{
    Enemy, Stranger, Friend, Soulmate
}

//public enum NewWolfType
//{
//    Herbivore,
//    Carnivore
//}

public enum TargetType
{
    Player,
    Home,
    Foods,
    Enemies
}

public enum ActionType
{
    Move, Eat, Rest, GoToPlayer, Attack, MoveOrders, Rotation, Defend
}

public class NewWolfAgent : Agent
{
    [Header("Creature Parameters")]
    //public NewWolfType newWolfType;
    public Transform pivotTransform; // 임시 위치 기준점, 트레이닝룸 위치
    public Transform targetPlayer;
    public Transform targetHome;
    //public Transform targetFood;
    //public Transform targetEnemy;
    private Transform target;

    [Header("Creature Points (100 Max)")]
    //public float MaxEnergy;
    public float MaxHp;
    public float MaxHungry;
    public float MaxFriendly;
    //public float MatureSize;
    //public float GrowthRate;
    public float EatingSpeed;
    public float MaxSpeed;
    public float AttackDamage;
    public float DefendDamage;
    public float Eyesight;

    public float moveForce;

    [Header("Monitoring")]
    //public float Energy;
    //public float Size;
    //public float Age;
    public float Hp;
    public float Hungry;
    public float Friendly;
    public string currentAction;

    //[Header("Child")]
    //public GameObject ChildSpawn;

    [Header("Species Parameters")]
    //public float AgeRate = .001f;


    /////////////////  Hide Parameters  /////////////////
    [HideInInspector] public PlayerRelation playerRelation;
    [HideInInspector] public TargetType targetType;

    
    // 랜덤 스폰하기 위한 임시적인 범위, 맵 20X20 공간에서만 가능
    private float minRange = -10f;  
    private float maxRange = 10f;

    // Penalty
    private float penalty = 0f;
    private float hpPenalty = 0f;
    private float hungryPenalty = 0f;
    private float friendlyPenalty = 0f;

    //private Vector2 bounds;
    private GameObject Environment;
    private Rigidbody agentRB;
    private float nextAction;
    private bool died;
    private RayPerception rayPer;    
    //private TerrariumAcademy academy;
    private int count;
    
    private void Awake()
    {
        InitializeAgent();
        AgentReset();
    }

    public override void AgentReset()
    {
        //Size = 1;
        //Energy = 1;
        //Age = 0;
        //bounds = GetEnvironmentBounds();
        //var x = Random.Range(-bounds.x, bounds.x);
        //var z = Random.Range(-bounds.y, bounds.y);
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 1f, Random.Range(minRange, maxRange));
        transform.position = randomPos + pivotTransform.position;
        //transform.position = new Vector3(x, 1, z);
        //TransformSize();
        currentAction = "Idle";
    }

    public override void AgentOnDone()
    {

    }

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();
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
        //AddVectorObs(Energy);
        //AddVectorObs(Size);
        //AddVectorObs(Age);
        AddVectorObs(Hp);
        AddVectorObs(Hungry);
        AddVectorObs(Friendly);
        //AddVectorObs(Float(CanReproduce));
        AddVectorObs(BoolToFloat(CanEat));
        AddVectorObs(BoolToFloat(CanRest));
        AddVectorObs(BoolToFloat(CanGoToPlayer));

        // TODO: 플레이어와의 거리 or 플레이어 위치 추가
    }

    private float BoolToFloat(bool val)
    {
        if (val) return 1.0f;
        else return 0.0f;
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Action Space 7 float (Ctl+G line 25)

        if (vectorAction[(int)ActionType.Move] > .5)
        {
            MoveAgent(vectorAction);
        }
        else if (vectorAction[(int)ActionType.Eat] > .5)
        {
            Eat();
        }
        else if (vectorAction[(int)ActionType.Rest] > .5)
        {
            Rest();
        }
        else if (vectorAction[(int)ActionType.GoToPlayer] > .5)
        {
            GoToPlayer();
        }
        else if (vectorAction[(int)ActionType.Attack] > .5)
        {
            Attack();
        }
        //Defend();
    }

    void Defend()
    {
        currentAction = "Defend";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    void Attack()
    {
        //float damage = 0f;
        currentAction = "Attack";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
        var vic = FirstAdjacent("herbivore").GetComponent<MLTestEnemy>();
        if (vic != null)
        {
            Debug.Log("herbivore enemy!");

            //if (vic.currentAction == "Defend")
            //{
                //damage = ((AttackDamage * Size) - (vic.DefendDamage * vic.Size)) / (Size * vic.Size);
            //}
            //else
            //{
                //damage = ((AttackDamage * Size) - (1 * vic.Size)) / (Size * vic.Size);
            //}
        }
        else
        {
            vic = FirstAdjacent("carnivore").GetComponent<MLTestEnemy>();
            if (vic != null)
            {
                Debug.Log("carnivore enemy!");

                //if (vic.currentAction == "Defend")
                //{
                //    damage = ((AttackDamage * Size) - (vic.AttackDamage * vic.Size)) / (Size * vic.Size);
                //}
                //else
                //{
                //    damage = ((AttackDamage * Size) - (vic.DefendDamage * vic.Size)) / (Size * vic.Size);
                //}
            }
            else return;
        }

        //if (damage > 0)
        //{
        //    vic.Energy -= damage;
        //    if (vic.Energy < 0)
        //    {
        //        AddReward(.25f);
        //    }
        //}
        //else if (damage < 0){
        //    Energy -= damage;
        //}
        //Energy -= .1f;

        vic.Hp -= AttackDamage;
        Hp -= vic.AttackDamage;
        if (vic.Hp <= 0)
        {
            AddReward(.25f);
        }
    }
        
    void Update()
    {
        // TODO: OutOfBounds or Dead를 DeadZone 알고리즘으로 바꾸기
        //if (OutOfBounds)    
        //{
        //    AddReward(-1f);
        //    Done();
        //    return;
        //}
        //if (Buried)
        //{
        //    Done();
        //}
        if (Dead) return;
        //if (CanGrow) Grow();        
        //if (CanReproduce) Reproduce();        
        //Age += AgeRate; 
        MonitorLog();
    }

    public void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > nextAction)
        {
            currentAction = "Deciding";
            RequestDecision();
        }
    }

    public void MonitorLog()
    {
        Monitor.Log("Action", currentAction, transform);
        //Monitor.Log("Size", Size / MatureSize, transform);
        //Monitor.Log("Energy", Energy / MaxEnergy, transform);
        //Monitor.Log("Age", Age / MatureSize, transform);
        Monitor.Log("Hp", Hp / MaxHp, transform);
        Monitor.Log("Hungry", Hungry / MaxHungry, transform);
        Monitor.Log("Friendly", Friendly / MaxFriendly, transform);
    }

    //public bool OutOfBounds
    //{
    //    get
    //    {
    //        if (transform.position.y < 0) return true;
    //        if (transform.position.x > bounds.x ||
    //            transform.position.x < -bounds.x ||
    //            transform.position.y > bounds.y ||
    //            transform.position.y < -bounds.y)
    //            return true;
    //        return false;
    //    }
    //}
    
    //void TransformSize()
    //{
    //    transform.localScale = Vector3.one * Mathf.Pow(Size,1/3);
    //}

    //bool CanGrow
    //{
    //    get
    //    {
    //        return Energy > ((MaxEnergy / 2) + 1);
    //    }
    //}

    bool CanEat
    {
        get
        {
            //if(newWolfType == NewWolfType.Herbivore)
            //{
                if (FirstAdjacent("food") != null) return true;
            //}
            return false;
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
        var colliders = Physics.OverlapSphere(transform.position, 1f);
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

    //bool CanReproduce
    //{
    //    get
    //    {
    //        if (Size >= MatureSize && CanGrow) return true;
    //        else return false;
    //    }
    //}

    bool Dead
    {
        get
        {
            if (died) return true;
            
            if (0 >= Hp)    // TODO: 사망 조건 추가
            {
                // TODO: 사망 후처리 추가
                currentAction = "Dead";            
                died = true;
                AddReward(-1f);
                Done();
                return true;
            }

            return false;
        }
    }

    //bool Buried
    //{
    //    get
    //    {
    //        Energy -= AgeRate;
    //        return Energy < 0;
    //    }
    //}

    //void Grow()
    //{
    //    if (Size > MatureSize) return;
    //    Energy = Energy / 2;
    //    Size += GrowthRate * Random.value;
    //    nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    //    currentAction ="Growing";
    //    TransformSize();
    //}

    //void Reproduce()
    //{
    //    if (CanReproduce)
    //    {
    //        var vec = Random.insideUnitCircle * 5;
    //        var go = Instantiate(ChildSpawn, new Vector3(vec.x, 0, vec.y), Quaternion.identity, Environment.transform);
    //        go.name = go.name + (count++).ToString();
    //        var ca = go.GetComponent<CreatureAgent>();
    //        ca.AgentReset();
    //        Energy = Energy / 2;
    //        AddReward(.2f);            
    //        currentAction ="Reproducing";
    //        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    //    }
    //}

    public void Eat()
    {
        if (CanEat)
        {
            //if (newWolfType == NewWolfType.Herbivore)
            //{
                var adj = FirstAdjacent("food");
                if (adj != null)
                {
                    // TODO: food (target) 먹었을 때 추가할 거

                    //var creature = adj.GetComponent<Plant>();
                    //var consume = Mathf.Min(creature.Energy, 5);
                    //creature.Energy -= consume;
                    //if (creature.Energy < .1) Destroy(adj);
                    //Energy += consume;
                    AddReward(.1f);
                    nextAction = Time.timeSinceLevelLoad + (25 / EatingSpeed);
                    currentAction = "Eating";
                }
            //}
        }
    }

    public void Rest()
    {

    }

    public void GoToPlayer()
    {

    }
    
    public void MoveAgent(float[] act)
    {        
        var rotate = Mathf.Clamp(act[(int)ActionType.Rotation], -1f, 1f);
        if(act[(int)ActionType.MoveOrders] > .5f)
        {            
            transform.position += transform.forward;
        }
        //Energy -= .01f;
        transform.Rotate(transform.up, rotate * 25f);        
        currentAction = "Moving";
        nextAction = Time.timeSinceLevelLoad + (25 / MaxSpeed);
    }

    private Vector2 GetEnvironmentBounds()
    {
        Environment = transform.parent.gameObject;
        var xs = Environment.transform.localScale.x;
        var zs = Environment.transform.localScale.z;
        return new Vector2(xs, zs) * 10;
    }
}
