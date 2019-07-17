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
    private GameObject basic_rock;
    [SerializeField]
    private GameObject fract_rock;
    [SerializeField]
    private GameObject effect_rock;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip mining_sound;
    [SerializeField]
    private AudioClip crash_sound;


    public void Mining()
    {
        audioSource.clip = mining_sound;
        audioSource.Play();

        var clone = Instantiate(effect_rock, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        audioSource.clip = crash_sound;
        audioSource.Play();

        col.enabled = false;
        Destroy(basic_rock);
        fract_rock.SetActive(true);
        Destroy(fract_rock, destroyTime);
    }
}
