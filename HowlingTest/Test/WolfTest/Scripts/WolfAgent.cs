using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : MonoBehaviour
{
    public enum WolfState { idle, walk, trace, escape, jump, die,   // 상태
                            attack, growl, howl,                    // 전투
                            eat, sit, lie_down, stretch,            // 생활
                            bite, lick, gift, rub                   // 상호작용
                          }
    public WolfState state = WolfState.idle;

    Transform target;
    NavMeshAgent agent;

    public float lookRadius = 10f;

    // 스테이터스 - 체력, 허기, 친밀도
    public float hp = 100f;
    public float hunger = 0f;
    public float friendship = 0f;

    // 무게
    public float mass = 45f;

    // 속도, 가속도
    public float velocity = 0.0f;
    public float walkAcc = 0.7f;
    public float runAcc = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= lookRadius)
        {
            agent.SetDestination(target.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
