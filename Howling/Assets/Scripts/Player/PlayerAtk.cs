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

        private Animator animator;
        private TutorialController tutorialController;

        // Start is called before the first frame update
        void Awake()
        {
            enemyMask = 1 << LayerMask.NameToLayer("EnemyCollider");
            enemyMask |= 1 << LayerMask.NameToLayer("EnemyCollider");
            currentAtkTime = 0;
            isAtk = false;
            animator = GetComponent<Animator>();
            tutorialController = FindObjectOfType<TutorialController>();
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetBool("Attack", false);
            if (currentAtkTime <= atkTime)
                currentAtkTime++;
            else
            {
                if (Input.GetMouseButtonDown(0) && Slot.isSlotClick == false)
                {
                    if (tutorialController.currentShow > 4)
                    {
                    AttackCheck();
                    currentAtkTime = 0;
                    animator.SetBool("Attack", true);

                    tutorialController.isPlayerAttack = true;
                    }
                }
            }

        }

        private void AttackCheck()
        {
            Collider[] enemys = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, enemyMask);
            foreach (Collider enemy in enemys) Attack(enemy.gameObject);
            foreach (Collider rock in enemys) Mining(rock.gameObject);
        }

        private void Attack(GameObject obj)
        {
            Enemy enemy = obj.GetComponentInParent<Enemy>();
            enemy.DecreaseHp(atk);
            EnemyExplosion enemyExplosion = enemy.GetComponentInChildren<EnemyExplosion>();
            enemyExplosion.isEnemyAtked = true;
        }

        private void Mining(GameObject obj)
        {
            Rock rock = obj.GetComponentInParent<Rock>();
            rock.Mining();
        }

    }
}