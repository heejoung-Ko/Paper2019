using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public enum CreatureType
{
    Herbivore,
    Carnivore
}
public class CreatureAgent : Agent
{
    [Header("Creature Type")]
    public CreatureType creatureType;
    [Header("Creature Points (100 Max)")]
    public float maxEnergy;
    public float matureSize;
    public float growthRate;
    public float eatingSpeed;
    public float maxSpeed;
    public float attackDamage;
    public float defendDamage;
    public float eyesight;

    [Header("Monitoring")]
    public float energy;
    public float size;
    public float age;
    public string currentAction;
    
    [Header("Child")]
    public GameObject childSpawn;

    [Header("Species Parameters")]    
    public float ageRate = .001f;

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
        AgentReset();
    }

    public override void AgentReset()
    {
        size = 1;
        energy = 1;
        age = 0;
        //bounds = GetEnvironmentBounds();
        //var x = Random.Range(-bounds.x, bounds.x);
        //var z = Random.Range(-bounds.y, bounds.y);
        //transform.position = new Vector3(x, 1, z);
        TransformSize();
        InitializeAgent();
    }

    public override void AgentOnDone()
    {

    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();
        currentAction = "Idle";
    }

    public override void CollectObservations()
    {
        float rayDistance = eyesight;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "plant", "herbivore", "carnivore" };
        
        // 에이전트 관찰의 일부로 사용할 인식 벡터를 생성한다
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        Vector3 localVelocity = transform.InverseTransformDirection(agentRB.velocity);

        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(energy);
        AddVectorObs(size);
        AddVectorObs(age);
        AddVectorObs(Float(CanEat));
        AddVectorObs(Float(CanReproduce));
    }

    private float Float(bool val)
    {
        if (val) return 1.0f;
        else return 0.0f;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Action Space 7 float
        // 0 = Move
        // 1 = Eat
        // 2 = Reproduce
        // 3 = Attack
        // 4 = Defend
        // 5 = move orders
        // 6 = rotation

        if (vectorAction[0] > .5)
        {
            MoveAgent(vectorAction);
        }
        else if (vectorAction[1] > .5)
        {
            Eat();
        }
        else if (vectorAction[2] > .5)
        {
            Reproduce();
        }
        //else if (vectorAction[3] > .5)
        //{
        //    Attack();
        //}
        //else if (vectorAction[4] > .5)
        //{
        //    Defend();
        //}
    }

    void Defend()
    {
        currentAction = "Defend";
        nextAction = Time.timeSinceLevelLoad + (25 / maxSpeed);
    }

    void Attack()
    {
        float damage = 0f;
        var vic = FirstAdjacent("herbivore").GetComponent<CreatureAgent>();
        if (vic != null)
        {
            if (vic.currentAction == "Defend")
            {
                damage = ((attackDamage * size) - (vic.defendDamage * vic.size)) / (size * vic.size);
            }
            else
            {
                damage = ((attackDamage * size) - (1 * vic.size)) / (size * vic.size);
            }
        }
        else
        {
            vic = FirstAdjacent("carnivore").GetComponent<CreatureAgent>();
            if (vic != null)
            {
                if (vic.currentAction == "Defend")
                {
                    damage = ((attackDamage * size) - (vic.attackDamage * vic.size)) / (size * vic.size);
                }
                else
                {
                    damage = ((attackDamage * size) - (vic.defendDamage * vic.size)) / (size * vic.size);
                }
            }
        }
        if(damage > 0)
        {
            vic.energy -= damage;
            if (vic.energy < 0)
            {
                AddReward(.25f);
            }
        }
        else if(damage < 0){
            energy -= damage;
        }
        energy -= .1f;
    }
        
    void Update()
    {
        //if (OutOfBounds)
        //{
        //    Debug.Log("Out Of Bounds!!");
        //    AddReward(-1f);
        //    Done();
        //    return;
        //}
        if (Buried)
        {
            Debug.Log("Buried!!");
            Done();
        }
        if (Dead) return;
        if (CanGrow) Grow();        
        if (CanReproduce) Reproduce();        
        age += ageRate; 
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
        Monitor.Log("Size", size / matureSize, transform);
        Monitor.Log("Energy", energy / maxEnergy, transform);
        Monitor.Log("Age", age / matureSize, transform);
    }

    //public bool OutOfBounds
    //{
    //    get
    //    {
    //        Debug.Log(bounds.x);
    //        Debug.Log(bounds.y);
    //        Debug.Log(transform.position.x);
    //        Debug.Log(transform.position.y);

    //        if (transform.position.y < 0) return true;
    //        if (transform.position.x > bounds.x ||
    //            transform.position.x < -bounds.x ||
    //            transform.position.y > bounds.y ||
    //            transform.position.y < -bounds.y)
    //            return true;
    //        return false;
    //    }
    //}
    
    void TransformSize()
    {
        transform.localScale = Vector3.one * Mathf.Pow(size,1/3);
    }

    bool CanGrow
    {
        get
        {
            return energy > ((maxEnergy / 2) + 1);
        }
    }

    bool CanEat
    {
        get
        {
            if(creatureType == CreatureType.Herbivore)
            {
                if (FirstAdjacent("plant") != null) return true;
            }
            return false;
        }
    }

    private GameObject FirstAdjacent(string tag)
    {
        var colliders = Physics.OverlapSphere(transform.position, 1.2f * size);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == tag)
            {
                return collider.gameObject;
            }
        }
        return null;
    }

    bool CanReproduce
    {
        get
        {
            if (size >= matureSize && CanGrow) return true;
            else return false;
        }
    }

    bool Dead
    {
        get
        {
            if (died) return true;
            if (age > matureSize )
            {

                currentAction = "Dead";            
                died = true;
                energy = size;  //creature size is converted to energy
                AddReward(.2f);
                Done();
                return true;
            }
            return false;
        }
    }

    bool Buried
    {
        get
        {
            energy -= ageRate;
            return energy < 0;
        }
    }

    void Grow()
    {
        if (size > matureSize) return;
        energy = energy / 2;
        size += growthRate * Random.value;
        nextAction = Time.timeSinceLevelLoad + (25 / maxSpeed);
        currentAction ="Growing";
        TransformSize();
    }

    void Reproduce()
    {
        if (CanReproduce)
        {
            Environment = transform.parent.gameObject;

            var vec = Random.insideUnitCircle * 5;
            var go = Instantiate(childSpawn, new Vector3(vec.x, 0, vec.y), Quaternion.identity, Environment.transform);
            go.name = go.name + (count++).ToString();
            var ca = go.GetComponent<CreatureAgent>();
            ca.AgentReset();
            energy = energy / 2;
            AddReward(.2f);            
            currentAction ="Reproducing";
            nextAction = Time.timeSinceLevelLoad + (25 / maxSpeed);
        }
    }

    public void Eat()
    {
        if (CanEat)
        {
            if (creatureType == CreatureType.Herbivore)
            {
                var adj = FirstAdjacent("plant");
                if (adj != null)
                {
                    var creature = adj.GetComponent<Plant>();
                    var consume = Mathf.Min(creature.energy, 5);
                    creature.energy -= consume;
                    if (creature.energy < .1) Destroy(adj);
                    energy += consume;
                    AddReward(.1f);
                    nextAction = Time.timeSinceLevelLoad + (25 / eatingSpeed);
                    currentAction = "Eating";
                }
            }
        }
    }
    
    public void MoveAgent(float[] act)
    {        
        var rotate = Mathf.Clamp(act[6], -1f, 1f);
        if(act[5] > .5f)
        {            
            transform.position = transform.position + transform.forward;
        }
        energy -= .01f;
        transform.Rotate(transform.up, rotate*25f);        
        currentAction = "Moving";                
        nextAction = Time.timeSinceLevelLoad + (25 / maxSpeed);
    }

    //private Vector2 GetEnvironmentBounds()
    //{
    //    Environment = transform.parent.gameObject;
    //    var xs = Environment.transform.localPosition.x;
    //    var zs = Environment.transform.localPosition.z;
    //    return new Vector2(xs, zs) * 10;
    //}

    
}
