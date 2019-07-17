using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class PlayerHandController : MonoBehaviour
    {
        // 현재 장착된 Hand형 타입 무기.
        [SerializeField]
        private PlayerHand currentHand;

        private bool isAttack = false;
        private bool isSwing = false;

        private RaycastHit hitInfo;

        // Update is called once per frame
        void Update()
        {
            TryAttack();
        }

        private void TryAttack()
        {
            if (Input.GetButton("Fire1"))
            {
                if (!isAttack)
                {
                    StartCoroutine(AttackCoroutine());
                }
            }
        }

        IEnumerator AttackCoroutine()
        {
            Debug.Log("start attack coroutine");
            isAttack = true;
            //currentHand.anim.SetTrigger("Attack");

            yield return new WaitForSeconds(currentHand.attackActiveDelay);
            isSwing = true;

            StartCoroutine(HitCoroutine());

            yield return new WaitForSeconds(currentHand.attackCancleDelay);
            isSwing = false;


            yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackActiveDelay - currentHand.attackCancleDelay);
            isAttack = false;
        }

        IEnumerator HitCoroutine()
        {
            while (isSwing)
            {
                if (CheckObject())
                {
                    if(hitInfo.transform.tag == "rock")
                    {
                        hitInfo.transform.GetComponent<Rock>().Mining();
                    }
                    isSwing = false;
                    Debug.Log(hitInfo.transform.name);
                }
                yield return null;
            }
        }

        private bool CheckObject()
        {
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
            {
                return true;
            }
            return false;
        }
    }
}