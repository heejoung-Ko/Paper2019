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

    //private bool dead;
    //private MLTestManager manager;

    private void Start()
    {
        //dead = false;
        //manager = base.GetComponent<MLTestManager>();
    }

    void Update()
    {
        if (Hp <= 0)
        {
            //if (tag == "herbivore") manager.herbivoreRespawn = true;
            //else if (tag == "carnivore") manager.carnivoreRespawn = true;
            Invoke("DropItem", 5f);
            Destroy(gameObject, 5f);
            return;
        }
    }

    //private void FixedUpdate()
    //{
    //    /*if (!dead)*/ Move();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            //dead = true;
            direction.y += 100;
            if (direction.y >= 360) direction.y = 0;
            Mathf.Clamp(direction.y, 0, 360);
            transform.Rotate(0, direction.y, 0);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("dead"))
    //    {
    //        dead = false;
    //    }
    //}

    private void Move()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    void DropItem()
    {
        Debug.Log("Drop Item!!!");
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
}
