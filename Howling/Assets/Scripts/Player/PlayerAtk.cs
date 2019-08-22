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

        private Animator animator;
        private TutorialController tutorialController;

        public PlayerHand playerHand;

        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        private LayerMask buildingMask;

        // Start is called before the first frame update
        void Awake()
        {
            //enemyMask = 1 << LayerMask.NameToLayer("EnemyCollider");
            //enemyMask |= 1 << LayerMask.NameToLayer("EnemyCollider");
            currentAtkTime = 0;
            isAtk = false;
            animator = GetComponent<Animator>();
            // tutorialController = FindObjectOfType<TutorialController>();
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
                    //if (tutorialController.currentShow > 4)
                    {
                        animator.SetBool("Attack", true);

                        AttackCheck();
                        currentAtkTime = 0;
                        //tutorialController.isPlayerAttack = true;
                    }
                }
            }

        }

        private void AttackCheck()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, enemyMask);
            foreach (Collider enemy in colliders) Attack(enemy.gameObject);

            if (playerHand.getAtk() > 1)
            {
                colliders = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, buildingMask);
                foreach (Collider building in colliders) building.GetComponent<Building>().Hittied();
            }
            // foreach (Collider resource in colliders) Gathering(resource.gameObject);
        }

        private void Attack(GameObject obj)
        {
            //if (obj.activeSelf == false) return;
            Enemy enemy = obj.GetComponentInParent<Enemy>();
            enemy.DecreaseHp(playerHand.getAtk());
            inventory.useSelectItem();
        }

        private void Gathering(GameObject obj)
        {
            if (obj.CompareTag("tree") || obj.CompareTag("rock"))
            {
                Resource resource = obj.GetComponentInParent<Resource>();
                resource.Gathering();
            }
        }

        public void setDrink()
        {
            animator.SetTrigger("Drink");
        }

        public bool isDrink()
        {

            Debug.Log(animator.GetCurrentAnimatorStateInfo(1).IsName("Drink"));
            return animator.GetCurrentAnimatorStateInfo(1).IsName("Drink");
        }
    }
}