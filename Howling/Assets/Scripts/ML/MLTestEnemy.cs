using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestEnemy : MonoBehaviour
{
    [HideInInspector] public GameObject instance;
    public GameObject dropItem;
    public float Hp;
    public float AttackDamage;
    public Vector3 direction;
    public float velocity;

    private bool dead;

    private void Start()
    {
        dead = false;
    }

    void Update()
    {
        if (0 >= Hp)
        {
            Invoke("DropItem", 5f);
            Destroy(gameObject, 5f);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!dead) Move();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            dead = true;
            direction.x += 0.1f;
            if (direction.x >= 1) direction.x = 0;
            Mathf.Clamp(direction.x, 0, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            dead = false;
        }
    }

    private void Move()
    {
        transform.position = new Vector3(transform.position.x + direction.x * velocity * Time.deltaTime,
                                                    transform.position.y,
                                                    transform.position.z + direction.z * velocity * Time.deltaTime);

        Quaternion newRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 5.0f);
    }

    void DropItem()
    {
        Debug.Log("Drop Item!!!");
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
}
