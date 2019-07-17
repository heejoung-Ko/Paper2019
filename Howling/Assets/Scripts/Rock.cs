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

    public void Mining()
    {
        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        col.enabled = false;
        Destroy(basic_rock);
        fract_rock.SetActive(true);
        Destroy(fract_rock, destroyTime);
    }
}
