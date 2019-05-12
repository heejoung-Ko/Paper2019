using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    public ParticleSystem psExplosion;
    public bool isEnemyAtked = false;

    private void Update()
    {
        if (isEnemyAtked)
        {
            isEnemyAtked = false;
            psExplosion.Play();
        }
    }
}
