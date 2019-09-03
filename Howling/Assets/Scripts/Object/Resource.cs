using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField]
    private int initHp;
    private int hp;

    [SerializeField]
    private float destroyTime; // 파편 제거 시간
    [SerializeField]
    private float respawnTime; // 리스폰에 필요한 시간

    //private float respawnTimeCount; // 리스폰 카운트

    [SerializeField]
    private CapsuleCollider col;
    [SerializeField]
    private int count1; // 자원 등장 갯수
    [SerializeField]
    private int count2; // 자원 등장 갯수
    [SerializeField]
    private int hiddenCout; // 자원 등장 갯수

    [SerializeField]
    private GameObject basic_resource;
    [SerializeField]
    private GameObject fract_resource;
    [SerializeField]
    private GameObject effect_resource;
    [SerializeField]
    private GameObject item_resource1;
    [SerializeField]
    private GameObject item_resource2;
    [SerializeField]
    private GameObject item_hidden;

    [SerializeField]
    private string mining_sound;
    [SerializeField]
    private string crash_sound;

    private float init_posY;

    private void Start()
    {
        hp = initHp;
        init_posY = transform.position.y;
    }

    public IEnumerator Gathering(float attackActiveDelay)
    {
        yield return new WaitForSeconds(attackActiveDelay);

        for (float i = 0f; i < 0.5; i += Time.deltaTime) ;

        SoundManager.instance.PlaySE(mining_sound);

        var clone = Instantiate(effect_resource, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();

        yield return null;
    }

    private void Destruction()
    {
        basic_resource.SetActive(false);

        SoundManager.instance.PlaySE(crash_sound);

        Vector3 position = new Vector3(col.bounds.center.x, col.bounds.center.y + 1f, col.bounds.center.z);

        for (int i = 0; i < count1; i++)
        {
            Instantiate(item_resource1, col.bounds.center, Quaternion.identity);
        }
        for (int i = 0; i < count2; i++)
        {

            Instantiate(item_resource2, col.bounds.center, Quaternion.identity);
        }

        int ran = Random.Range(0, 10);
        if(ran < hiddenCout)
            Instantiate(item_hidden, col.bounds.center, Quaternion.identity);

        var fract_clone = Instantiate(fract_resource, col.bounds.center, Quaternion.identity);
        fract_clone.SetActive(true);
        Destroy(fract_clone, destroyTime);

        col.enabled = false;

        StartCoroutine("Respawn");
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        transform.position = new Vector3(transform.position.x, init_posY, transform.position.y);

        hp = initHp;

        col.enabled = true;

        basic_resource.SetActive(true);
    }

}
