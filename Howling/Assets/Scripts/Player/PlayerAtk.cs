using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class PlayerAtk : MonoBehaviour
    {
        private const float atkPos = 1f;
        private float atkRange = 1f;
        private LayerMask enemyMask;

        private int atkTime = 30;
        private int currentAtkTime = 0;
        private bool isAtk = false;
        private int atk = 10;

        // Start is called before the first frame update
        void Awake()
        {
            enemyMask = 1 << LayerMask.NameToLayer("EnemyCollider");
            enemyMask |= 1 << LayerMask.NameToLayer("EnemyCollider");
            currentAtkTime = 0;
            isAtk = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentAtkTime <= atkTime)
                currentAtkTime++;
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    AttackCheck();
                    currentAtkTime = 0;
                }
            }

        }

        private void AttackCheck()
        {
            Collider[] enemys = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, enemyMask);
            foreach (Collider enemy in enemys) Attack(enemy.gameObject);
        }

        private void Attack(GameObject obj)
        {
            Enemy enemy = obj.GetComponentInParent<Enemy>();
            enemy.DecreaseHp(atk);
        }
    }
}