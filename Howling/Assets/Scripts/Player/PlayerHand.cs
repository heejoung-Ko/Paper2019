using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class PlayerHand : MonoBehaviour
    {
        public string handName;     // 종류 구분
        public float range;         // 공격 범위 
        public int damage;          // 공격력
        public float workSpeed;   // 작업 속도
        public float attackDelay;   // 공격 딜레이
        public float attackActiveDelay;  // 공격 활성화 시점.
        public float attackCancleDelay;  // 공격 비활성화 시점.

        public Animator anim;

        // 손 위치
        public GameObject HandPosition;

        // 현재 착용 중인 도구 타입
        // -1이면 None
        public int EquipToolType;

        [SerializeField]
        private GameObject Player;

        private void Start()
        {
            int count = transform.GetChild(0).childCount;

            EquipToolType = -1;
        }

        private void Update()
        {
            transform.position = HandPosition.transform.position;
            transform.rotation = HandPosition.transform.rotation;
        }

        public void swapTools(int n)
        {
            if (EquipToolType == n)
            {
                anim.SetBool("Swap", true);
                return;
            }
            // 맨 손일 때
            if (n == -1)
            { 
                transform.GetChild(0).transform.GetChild(EquipToolType).gameObject.SetActive(false);
                EquipToolType = n;
            }
            else
            {
                if(EquipToolType != -1)
                    transform.GetChild(0).transform.GetChild(EquipToolType).gameObject.SetActive(false);
                EquipToolType = n;
                transform.GetChild(0).transform.GetChild(EquipToolType).gameObject.SetActive(true);
            }
            anim.SetInteger("Tool", EquipToolType);
            anim.SetBool("Swap", true);
        }

        public bool EquipAxe()
        {
            if (EquipToolType == 0)
                return true;
            else return false;
        }

        public bool EquipPick()
        {
            if (EquipToolType == 1)
                return true;
            else return false;
        }
    }
}
