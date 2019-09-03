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
        public LayerMask wolfMask;
        public LayerMask resourceMask;

        private bool isAtk = false;

        private Animator animator;

        public PlayerHand playerHand;

        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        private LayerMask buildingMask;

        // Start is called before the first frame update
        void Awake()
        {
            isAtk = false;
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("Idle_None") || animator.GetCurrentAnimatorStateInfo(1).IsName("Idle_Tool"))
            {
                if (Input.GetMouseButtonDown(0) && Slot.isSlotClick == false)
                {
                    {
                        animator.SetTrigger("Attack");
                        isAtk = true;

                        AttackCheck();
                    }
                }
            }

        }

        private void AttackCheck()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, enemyMask);
            if (colliders.Length == 0)
            {
                Collider[] wolfCollider = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, wolfMask);
                if (wolfCollider.Length != 0)
                {
                    wolfCollider[0].GetComponent<WolfAgent>().decreaseStatusByPlayerAtk(playerHand.getAtk());
                }
            }
            foreach (Collider enemy in colliders) Attack(enemy.gameObject);

            if (playerHand.getAtk() > 1)
            {
                colliders = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, buildingMask);
                foreach (Collider building in colliders) building.GetComponent<Building>().Hittied();
            }

            Collider[] resources = Physics.OverlapSphere(transform.position + transform.forward * atkPos, atkRange, resourceMask);
            foreach (Collider resource in resources) Gathering(resource.gameObject);
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
            if (obj.transform.CompareTag("tree") && playerHand.EquipAxe())
            {
                StartCoroutine( obj.transform.GetComponent<Resource>().Gathering(playerHand.attackActiveDelay));

                inventory.useSelectItem();
            }
            else if (obj.transform.CompareTag("rock") && playerHand.EquipPick())
            {
                StartCoroutine(obj.transform.GetComponent<Resource>().Gathering(playerHand.attackActiveDelay));
                inventory.useSelectItem();
            }
        }

        public void setDrink()
        {
            animator.SetTrigger("Drink");
        }

        public bool isDrink()
        {
            return animator.GetCurrentAnimatorStateInfo(1).IsName("Drink");
        }
    }
}