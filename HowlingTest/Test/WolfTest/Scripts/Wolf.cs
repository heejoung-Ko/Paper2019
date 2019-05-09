using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class WolfAgent : Agent
{
    public enum WolfState { idle, walk, trace, escape, jump, die,   // 상태
                            attack, growl, howl,                    // 전투
                            eat, sit, lie_down, stretch,            // 생활
                            bite, lick, gift, rub                   // 상호작용
                          }
    public WolfState state = WolfState.idle;

    public Vector3 direction;

    public float actionTime = 0.0f;
    public float nowTime = 0.0f;

    private Transform target;
    private NavMeshAgent nmaAgent;
    // NavMeshAgent 게임 오브젝트의 Navigation 정보를 분석하여 목표물을 추적하게 하는 컴포넌트


    // 스테이터스 - 체력, 허기, 친밀도
    public float healthpoint = 100f;
    public float hunger = 0f;
    public float friendship = 0f;

    // 무게
    public float mass = 45f;

    // 속도, 가속도
    public float velocity = 0.0f;
    public float walkAcc = 0.7f;
    public float runAcc = 1.5f;

    // 인식 범위
    public float cognizanceDist = 10f;
    public float distance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //        target = PlayerManager.instance.player.transform;
        target = GameObject.Find("enemy").GetComponent<Transform>();
        nmaAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position);

        switch (state)
        {
            case WolfState.idle:
                {
                    nowTime += Time.deltaTime;
                    if (nowTime >= actionTime)
                    {
                        NextAction();
                        state = WolfState.walk;
                        this.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                        direction = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
                    }
                }
                break;

            case WolfState.walk:
                {
                    velocity = velocity + walkAcc * Time.deltaTime;                // 속도 계산
                    float newX = transform.position.x + direction.x * velocity;
                    newX = Mathf.Clamp(newX, -5.0f, 5.0f);
                    float newZ = transform.position.z + direction.z * velocity;
                    newZ = Mathf.Clamp(newZ, -5.0f, 5.0f);
                    this.transform.position = new Vector3(newX, transform.position.y, newZ);

                    // 걷는 중 타겟과의 거리가 가까워 지면
                    if (healthpoint >= 50f && distance <= cognizanceDist)
                    {
                        // 타겟으로 지정하고 잡으러간다
                        nmaAgent.SetDestination(target.position);

                        nmaAgent = GetComponent<NavMeshAgent>();

                        state = WolfState.trace;
                    }


                    nowTime += Time.deltaTime;
                    if (nowTime >= actionTime)
                    {
                        NextAction();
                        state = WolfState.idle;
                    }

                }
                break;

            case WolfState.trace:
                {
                    nmaAgent.destination = target.position;

                    if (distance <= nmaAgent.stoppingDistance)
                    {
                        state = WolfState.attack;
                    }

                }
                break;

            case WolfState.escape:
                {
                    // 집이나 플레이어를 목적지로 정하고 도주 ㅌㅌ
                    // 친밀도가 50이상이면 집과 플레이어중 가까운쪽으로 도주하는걸로
                    // target = GameObject.Find("Target 이름").GetComponent<Transform>();
                    if (friendship > 50f)
                    {
                        Transform home = GameObject.Find("Home").GetComponent<Transform>();
                        Transform player = GameObject.Find("Player").GetComponent<Transform>();

                        float homeDist = Vector3.Distance(home.position, transform.position);
                        float playerDist = Vector3.Distance(player.position, transform.position);

                        if(homeDist >= playerDist)
                        {
                            target = player;
                        }
                        else
                        {
                            target = home;
                        }
                    }
                    else
                    {
                        target = GameObject.Find("Home").GetComponent<Transform>();
                    }

                    nmaAgent.destination = target.position;
                    if (distance <= nmaAgent.stoppingDistance)
                    {
                        target = GameObject.Find("enemy").GetComponent<Transform>();
                        state = WolfState.idle;
                    }
                }
                break;

               // 점프 구현 해야하는건가??? 이동하면서 점프하는건가?? 제자리에서 하는건가?? 상의 해봐야겠음
            case WolfState.jump:
                break;

                // 늑대가 죽었다면.. 게임이 끝나야합니다..
            case WolfState.die:
                {

                }
                break;


            case WolfState.attack:
                {
                    FaceTarget();

                    // 공격하셈
                }
                break;

            case WolfState.growl:
                break;

            case WolfState.howl:
                break;

            case WolfState.eat:
                break;

            case WolfState.sit:
                break;

            case WolfState.lie_down:
                break;

            case WolfState.stretch:
                break;

            case WolfState.bite:
                break;

            case WolfState.lick:
                break;

            case WolfState.gift:
                break;

            case WolfState.rub:
                break;
            default:
                Debug.Log("not define wolf state!!");
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cognizanceDist);
    }

    void NextAction()
    {
        actionTime = Random.value * 5;
        nowTime = 0.0f;
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
