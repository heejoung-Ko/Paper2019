using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    private float destroyTime; // 파편 제거 시간

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
        SoundManager.instance.PlaySE(crash_sound);

        col.enabled = false;
        for (int i = 0; i < count; i++)
        {
            Instantiate(item_rock, basic_rock.transform.position, Quaternion.identity);
        }

        Destroy(basic_rock);
        fract_rock.SetActive(true);
        Destroy(fract_rock, destroyTime);
    }
}
