﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int initHp;

    private int hp;

    [SerializeField]
    private bool destroy;

    [SerializeField]
    private float destroyTime; // 파편 제거 시간
    [SerializeField]
    private float respawnTime; // 리스폰에 필요한 시간

    private float respawnTimeCount; // 리스폰 카운트

    [SerializeField]
    private SphereCollider col;
    [SerializeField]
    private int count; // 돌맹이 등장 갯수

    [SerializeField]
    private GameObject basic_rock;
    [SerializeField]
    private GameObject fract_rock;
    [SerializeField]
    private GameObject effect_rock;
    [SerializeField]
    private GameObject item_rock;

    [SerializeField]
    private string mining_sound;
    [SerializeField]
    private string crash_sound;

    private void Start()
    {
        respawnTimeCount = respawnTime;
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
    }

    public void Respawn()
    {
        Debug.Log("Respawn Rock");

        destroy = false;
        hp = initHp;
        respawnTimeCount = respawnTime;

        col.enabled = true;

        basic_rock.SetActive(true);
    }

    public void Mining()
    {
        SoundManager.instance.PlaySE(mining_sound);

        var clone = Instantiate(effect_rock, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        basic_rock.SetActive(false);

        SoundManager.instance.PlaySE(crash_sound);

        col.enabled = false;
        for (int i = 0; i < count; i++)
        {
            Instantiate(item_rock, col.transform.position, Quaternion.identity);
        }
        
        var fract_clone = Instantiate(fract_rock, col.bounds.center, Quaternion.identity);
        fract_clone.SetActive(true);
        Destroy(fract_clone, destroyTime);

        destroy = true;
    }
}
