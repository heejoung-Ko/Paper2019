using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestEnemy : MonoBehaviour
{
    public float Hp;
    public float AttackDamage;
    public Vector3 direction;
    public float velocity;

    private void Start()
    {

    }

    void Update()
    {
        if (0 >= Hp)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        Move();
    }

    // TOOD: DeadZone 닿으면 방향 바꾸기

    private void Move()
    {
        transform.position = new Vector3(transform.position.x + direction.x * velocity * Time.deltaTime,
                                                    transform.position.y,
                                                    transform.position.z + direction.z * velocity * Time.deltaTime);

        Quaternion newRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 5.0f);
    }
}
