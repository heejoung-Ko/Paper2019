using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class PlayerAtk : MonoBehaviour
    {
        private const float atkPos = 1f;
        private float atkRange = 1f;
        public LayerMask enemyMask;

        private int atkTime = 30;
        private int currentAtkTime = 0;
        private bool isAtk = false;
        private int atk = 10;

        private Animator animator;
        private TutorialController tutorialController;

        // Start is called before the first frame update
        void Awake()
        {
            //enemyMask = 1 << LayerMask.NameToLayer("EnemyCollider");
            //enemyMask |= 1 << LayerMask.NameToLayer("EnemyCollider");
            currentAtkTime = 0;
            isAtk = false;
            animator = GetComponent<Animator>();
            tutorialController = FindObjectOfType<TutorialController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentAtkTime <= atkTime)
                currentAtkTime++;
            else
            {
                if (Input.GetMouseButtonDown(0) && Slot.isSlotClick == false)
                {
                    if (tutorialController.currentShow > 4)
                    {
                        animator.SetBool("Attack", true);

                        AttackCheck();
                        currentAtkTime = 0;
                        tutorialController.isPlayerAttack = true;
                    }
                }
            }

        }

        private void AttackCheck()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, enemyMask);
            foreach (Collider enemy in colliders) Attack(enemy.gameObject);
            foreach (Collider resource in colliders) Gathering(resource.gameObject);
        }

        private void Attack(GameObject obj)
        {
            //if (obj.activeSelf == false) return;
            Enemy enemy = obj.GetComponentInParent<Enemy>();
            enemy.DecreaseHp(atk);
        }

        private void Gathering(GameObject obj)
        {
            if (obj.tag == "tree" || obj.tag == "rock")
            {
                Resource resource = obj.GetComponentInParent<Resource>();
                resource.Gathering();
            }
        }

        public void setDrink()
        {
            animator.SetTrigger("Drink");
        }
    }
}