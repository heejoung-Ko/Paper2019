using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public int m_EnemyNumber = 1;
    private Animator animator;

    // 적 상태 
    public enum EnemyState { idle, walk, trace, escape, attack, hit, die };
    public EnemyState state = EnemyState.idle;
    public EnemyState oldState;
    public EnemyType type;

    // 현재 타겟으로 설정된 객체
    [SerializeField] GameObject target = null;
    string targetTag = "target";
    string agentTag = "agent";

    public float atkPos = 0f;
    public float atkRange;
    public LayerMask targetMask;
    public LayerMask fireMask;

    float velocity = 0.0f;               // 속도
    [SerializeField] float walkAcc = 0.6f;               // 걸을 때 가속도
    [SerializeField] float runAcc = 1.3f;                // 뛸 때 가속도 (추적, 도주 상태 일 때)

    [SerializeField] float walkMaxVel = 6.0f;      // 걸을 때 최고 속도
    [SerializeField] float runMaxVel = 13.0f;      // 뛸 때 최고 속도

    static float keepTraceTime = 2.0f;   // 타겟이 인식 범위 밖으로 나갔을 때 추적 상태를 유지하는 시간
    static float keepEscapeTime = 2.0f;  // 타겟이 인식 범위 밖으로 나갔을 때 도주 상태를 유지하는 시간
    static float keepGoToSpawnPointTime = 8.0f;

    float nextStateTime = 0.0f;          // 다음 랜덤 상태까지 걸리는 총 시간
    float nowStateTime = 0.0f;           // 현재 기본 상태(idle, walk)에서 보낸 시간

    Vector3 direction;                   // 이동 방향

    public int hp;         // 체력
    public int atk;        // 공격력
    private int maxHp;

    public float invincibleTime = 1f; // 무적 시간
    public float currentInvincibleTime = 0f;

    [SerializeField]
    bool isDead;
    bool isAttack;


    [SerializeField]
    float atkTime = 0f;

    Quaternion oldRotation;

    public GameObject dropItem;
    private EnemiesManager enemiesManager;
    float nightSpeed = 1f;

    [SerializeField]
    private float detectDist;

    private bool isGoToSpawnPoint;
    private bool isTraceAtNight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        isDead = false;
        isAttack = false;

        StartCoroutine(Detect());
        StartCoroutine(Action());

        switch (type)
        {
            case EnemyType.RABBIT:
                name = "RABBIT";
                break;
            case EnemyType.FOX:
                name = "FOX";
                break;
            case EnemyType.DEER:
                name = "DEER";
                break;
            case EnemyType.BOAR:
                name = "BOAR";
                break;
            case EnemyType.BEAR:
                name = "BEAR";
                break;
            default: break;
        }
        maxHp = hp;
        isGoToSpawnPoint = false;
        isTraceAtNight = false;
    }

    private void Start()
    {
        StartCoroutine(Detect());
        StartCoroutine(Action());

        if (type == EnemyType.BEAR)
        {
            StartCoroutine(SetTraceAtNightBear());
        }
    }

    void TraceAtNight()
    {
        Debug.Log("TraceAtNight()");
        state = EnemyState.trace;
        target = enemiesManager.playerTarget;
        nextStateTime = 0.0f;
    }

    IEnumerator Detect()
    {
        while (!isDead)
        {
            //Debug.Log("감지");
            Collider[] fireColliders = Physics.OverlapSphere(transform.position, detectDist, fireMask);
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectDist, targetMask);

            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(transform.position, 10);

            if (fireColliders.Length != 0)
            {
                Debug.Log("enemy - campfire 닿음!!");
                target = fireColliders[0].gameObject;
                state = EnemyState.escape;
                nextStateTime = keepEscapeTime;
                nowStateTime = keepEscapeTime - 2f;
            }
            else if (state == EnemyState.idle || state == EnemyState.walk || state == EnemyState.hit)
            {
                if (target == null)
                {
                    if (colliders.Length != 0)
                    {
                        Debug.Log("찾았다!!!!!!!!1");
                        state = EnemyState.trace;
                        target = colliders[0].gameObject;
                        ChangeNextState();
                        nextStateTime = 0.0f;
                    }
                }
            }
            else if (state == EnemyState.trace || state == EnemyState.escape)
            {
                if (colliders.Length == 0)
                {
                    if (state == EnemyState.trace)
                        nextStateTime = keepTraceTime;
                    else if (state == EnemyState.escape)
                        nextStateTime = keepEscapeTime;
                }

            }
            //else if (state == EnemyState.escape)
            //    yield return new WaitForSeconds(keepEscapeTime - nowStateTime);

            yield return null;
        }
    }

    IEnumerator SetTraceAtNightBear()
    {
        while (!isDead)
        {
            if (enemiesManager.isBearTraceAtNight && !isGoToSpawnPoint)
            {
                if (!isTraceAtNight)
                {
                    isTraceAtNight = true;
                    StartCoroutine(StartGoToSpawnPointTimer());
                }
                TraceAtNight();
                yield return new WaitForSeconds(keepTraceTime);
            }
            else isGoToSpawnPoint = false;
            yield return null;
        }
    }

    IEnumerator StartGoToSpawnPointTimer()
    {
        yield return new WaitForSeconds(enemiesManager.stopTimeTraceAtNight);
        isGoToSpawnPoint = true;
        isTraceAtNight = false;
        GoToSpawnPoint();
    }

    void GoToSpawnPoint()
    {
        //state = EnemyState.trace;
        //Debug.Log("GoToSpawnPoint()");
        state = EnemyState.walk;
        target = enemiesManager.enemies[(int)type].enemiesSpawn[0].gameObject;
        direction = (target.transform.position - transform.position).normalized; // 타겟으로 향하는 방향
        direction.y = 0;
        target = null;
        nextStateTime = keepGoToSpawnPointTime;
        nowStateTime = 0f;
    }

    IEnumerator Action()
    {
        while (!isDead)
        {
            //Debug.Log("행동");
            switch (state)
            {
                case EnemyState.idle:
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isMoving", false);
                    Idle();
                    break;
                case EnemyState.walk:
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isMoving", true);
                    Walk();
                    break;
                case EnemyState.trace:
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isMoving", true);
                    Trace();
                    break;
                case EnemyState.escape:
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isMoving", true);
                    Escape();
                    break;
                case EnemyState.attack:
                    Attack();
                    break;
                case EnemyState.hit:
                    Hit();
                    break;
                case EnemyState.die:
                    animator.Play("die", 0);
                    Die();
                    break;
            }

            yield return null;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (target == null)
    //     {
    //         if (other.CompareTag(targetTag) || other.CompareTag(agentTag))
    //         {
    //             state = EnemyState.trace;
    //             target = other.gameObject;
    //             ChangeNextState();
    //             nextStateTime = 0.0f;
    //         }
    //     }
    // 
    //     if (other.gameObject.layer == 23)   // campfire
    //     {
    //         if (other.GetComponent<Campfire>().fireSize == FireSizeType.NONE) return;
    //         if (target == other.gameObject) return;
    // 
    //         target = other.gameObject;
    //         state = EnemyState.escape;
    //         nextStateTime = keepEscapeTime;
    //         nowStateTime = keepEscapeTime - 1f;
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (state == EnemyState.trace)
    //         nextStateTime = keepTraceTime;
    //     else if (state == EnemyState.escape)
    //         nextStateTime = keepEscapeTime;
    // }

    private void Move()
    {
        this.transform.position = new Vector3(this.transform.position.x + direction.x * velocity * Time.deltaTime * nightSpeed,
                                                    this.transform.position.y,
                                                    this.transform.position.z + direction.z * velocity * Time.deltaTime * nightSpeed);

        Quaternion newRotation = Quaternion.LookRotation(direction);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.deltaTime * 5.0f);
    }

    void ChangeNextState()
    {
        // 2 ~ 5 초 행동하고 다음 행동을 수행
        nextStateTime = 2 + Random.value * 3;
        nowStateTime = 0.0f;
        velocity = 0.0f;        // 속도 초기화
    }

    void Idle()
    {
        nowStateTime += Time.deltaTime;
        if (nowStateTime >= nextStateTime)
        {
            ChangeNextState();
            state = EnemyState.walk;
            direction = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
            return;
        }
    }

    void Walk()
    {
        velocity = velocity + walkAcc * Time.deltaTime;                // 속도 계산
        velocity = Mathf.Clamp(velocity, 0.0f, walkMaxVel);

        Move();

        nowStateTime += Time.deltaTime;
        if (nowStateTime >= nextStateTime)
        {
            ChangeNextState();
            animator.SetBool("isMoving", false);
            state = EnemyState.idle;
        }
        return;
    }

    private void OnDrawGizmos()
    {
        Vector3 atkPosition = new Vector3(transform.position.x, transform.position.y + atkPos, transform.position.z + transform.forward.z * atkPos);

        //Debug.Log(atkPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(atkPosition, atkRange);
    }

    void Trace()
    {
        if (enemiesManager.effectCameraController.isGameOver)
        {
            nowStateTime = nextStateTime;
            ChangeNextState();
            state = EnemyState.walk;
            target = null;
            return;
        }
        else if (enemiesManager.effectCameraController.isSleepInTent)
        {
            GoToSpawnPoint();
            return;
        }

        // 타겟과의 거리 계산
        float distance = Vector3.Distance(target.transform.position, transform.position);

        Vector3 atkPosition = new Vector3(transform.position.x, transform.position.y + atkPos, transform.forward.z * transform.position.z + atkPos);

        Collider[] targets = Physics.OverlapSphere(atkPosition, atkRange, targetMask);

        // 타겟이 공격범위 안에 들어왔을 때
        if (targets.Length != 0)
        {
            state = EnemyState.attack;
            nowStateTime = 0f;
            nextStateTime = 2f;
            velocity = 0f;
            oldRotation = transform.rotation;
            return;
        }

        direction = (target.transform.position - transform.position).normalized; // 타겟으로 향하는 방향
        direction.y = 0;

        velocity = velocity + runAcc * Time.deltaTime;                // 속도 계산
        velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);

        if (enemiesManager.isNight) nightSpeed = 1.5f;
        else nightSpeed = 1f;
        Move();

        // 타겟이 인식 범위 밖에 있을 경우
        if (nextStateTime == keepTraceTime)
        {
            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChangeNextState();
                state = EnemyState.walk;
                target = null;
            }
        }
    }

    void Escape()
    {
        if (target == null)
        {
            // Debug.Log("Enemy target is null.");
            direction = transform.forward;
        }
        direction = (target.transform.position - transform.position).normalized;  // 타겟으로 향하는 방향 
        direction.y = 0;
        direction *= -1;                                                // 타겟으로 향하는 역방향으로 전환

        velocity = velocity + runAcc * Time.deltaTime;                  // 속도 계산
        velocity = Mathf.Clamp(velocity, 0.0f, runMaxVel);

        if (enemiesManager.isNight) nightSpeed = 1.5f;
        else nightSpeed = 1f;
        Move();

        // 타겟이 인식 범위 밖에 있을 경우
        if (nextStateTime == keepTraceTime)
        {
            nowStateTime += Time.deltaTime;
            if (nowStateTime >= nextStateTime)
            {
                ChangeNextState();
                state = EnemyState.walk;
                target = null;
            }
        }
    }

    void Attack()
    {
        if (!isAttack)
        {
            animator.SetTrigger("attackTrigger");

            nextStateTime = atkTime;

            float tempAtk = atk;

            Vector3 atkPosition = new Vector3(transform.position.x, transform.position.y + atkPos, transform.forward.z * transform.position.z + atkPos);

            Collider[] targets = Physics.OverlapSphere(atkPosition, atkRange, targetMask);
            foreach (Collider t in targets)
            {
                if (t.gameObject.CompareTag(targetTag))
                {
                    if (enemiesManager.isNight) tempAtk = atk * 1.5f;
                    //GameObject.Find("UIManager").transform.GetChild(0).Find("Status").GetComponent<StatusController>().HitEnemy((int)tempAtk);
                    //GameObject.Find("CameraManager").GetComponent<EffectCameraController>().EffectCameraOn();
                    enemiesManager.statusController.HitEnemy((int)tempAtk);
                    enemiesManager.effectCameraController.EffectCameraOn();
                }
                else if (t.gameObject.CompareTag(agentTag))
                {
                    if (enemiesManager.isNight) tempAtk = atk * 1.5f;
                    WolfAgent wolf = t.GetComponent<WolfAgent>();
                    wolf.Hp -= tempAtk;
                }

                isAttack = true;
            }
        }

        nowStateTime += Time.deltaTime;
        if (nowStateTime >= nextStateTime)
        {
            state = EnemyState.trace;
            ChangeNextState();
            isAttack = false;
        }
        return;
    }


    void Hit()
    {
        if(currentInvincibleTime == 0)
            animator.SetTrigger("hitTrigger");
        currentInvincibleTime += Time.deltaTime;
        if (currentInvincibleTime > invincibleTime)
        {
            state = oldState;
            currentInvincibleTime = 0f;
        }
    }

    void Die()
    {
        if (!isDead)
        {
            enemiesManager.Die(gameObject);
            isDead = true;
        }
    }

    public void ResetForRespawn()
    {
        hp = maxHp;
        state = EnemyState.idle;
        oldState = EnemyState.idle;
    }

    public void DecreaseHp(int cnt)
    {
        if (state == EnemyState.die || state == EnemyState.hit) return;
        hp -= cnt;
        oldState = state;
        state = EnemyState.hit;
        velocity = 0;
        EnemyExplosion enemyExplosion = GetComponentInChildren<EnemyExplosion>();
        enemyExplosion.isEnemyAtked = true;
        Runaway(enemiesManager.playerTarget);
        //Debug.Log("데미지: " + cnt);

        if (hp <= 0)
        {
            //Debug.Log("주겄당!!");
            state = EnemyState.die;
            oldRotation = transform.rotation;
            animator.SetTrigger("dieTrigger");
        }
        //Debug.Log("enemy hp: " + hp);
    }

    public void DecreaseHpByWolf(int cnt)
    {
        if (state == EnemyState.die || state == EnemyState.hit) return;
        hp -= cnt;
        oldState = state;
        state = EnemyState.hit;
        velocity = 0;
        EnemyExplosion enemyExplosion = GetComponentInChildren<EnemyExplosion>();
        enemyExplosion.isEnemyAtked = true;
        enemiesManager.AtkReward(atk);
        Runaway(enemiesManager.wolfAgent.gameObject);

        if (hp <= 0)
        {
            //Debug.Log("주겄당!!");
            enemiesManager.DieReward();
            state = EnemyState.die;
            oldRotation = transform.rotation;
            animator.SetTrigger("dieTrigger");
        }
    }

    public void Runaway(GameObject go)
    {
        if (type == EnemyType.BOAR || type == EnemyType.BEAR) return;
        if (type == EnemyType.FOX || type == EnemyType.DEER)
        {
            if (hp >= maxHp * 0.3f) return;
        }
        target = go;
        state = EnemyState.escape;
    }

    void DropItem()
    {
        // Debug.Log("아이템 뿌린당!!!");
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
}
