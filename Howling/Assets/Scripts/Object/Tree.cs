using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField]
    private int initHp;
    private int hp;

    [SerializeField]
    private float initAge; // 나이만큼 아이템 뱉음
    private float age;
    private float maxAge;
    public float ageRate;

    [SerializeField]
    private bool destroy;

    [SerializeField]
    private float destroyTime; // 파편 제거 시간
    [SerializeField]
    private float respawnTime; // 리스폰에 필요한 시간

    private float respawnTimeCount; // 리스폰 카운트

    [SerializeField]
    private BoxCollider col;

    [SerializeField]
    private GameObject tree;
    [SerializeField]
    private GameObject cutting_tree;
    [SerializeField]
    private GameObject stump;
    [SerializeField]
    private GameObject effect;
    [SerializeField]
    private GameObject item;

    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falling_sound;

    private void Start()
    {
        maxAge = 10.0f;
        respawnTimeCount = respawnTime;
        age = Random.Range(1.0f, 3.0f);
        hp = initHp;
        //basic_rock_clone = Instantiate(basic_rock, col.bounds.center, Quaternion.identity);
        //fract_rock_clone = Instantiate(fract_rock, col.bounds.center, Quaternion.identity);
    }

    private void Update()
    {
        if (destroy)
        {
            if (respawnTimeCount <= 0)
            {
                respawnTimeCount = respawnTime;
                Respawn();
            }

            respawnTimeCount -= Time.deltaTime;
        }
        
        if(CanGrow)
        {
            TransformSize();
            age += ageRate;
        }

    }

    bool CanGrow
    {
        get
        {
            if (age <= maxAge) return true;
            else return false;
        }
    }

    void TransformSize()
    {
        transform.localScale = Vector3.one * (1.0f + (age * 0.1f));
    }

    public void Respawn()
    {
        Debug.Log("Respawn Tree");

        destroy = false;
        hp = initHp;
        age = initAge;
        respawnTimeCount = respawnTime;

        col.enabled = true;

        tree.SetActive(true);
        stump.SetActive(false);
    }

    public void Chopping()
    {
        SoundManager.instance.PlaySE(chop_sound);

        var clone = Instantiate(effect, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        tree.SetActive(false);
        stump.SetActive(true);

        SoundManager.instance.PlaySE(falling_sound);

        col.enabled = false;
        for (int i = 0; i < age; i++)
        {
            Instantiate(item, col.transform.position, Quaternion.identity);
        }

        var cutting_clone = Instantiate(cutting_tree, col.bounds.center, Quaternion.identity);
        cutting_clone.SetActive(true);
        Destroy(cutting_clone, destroyTime);

        destroy = true;
    }
}
