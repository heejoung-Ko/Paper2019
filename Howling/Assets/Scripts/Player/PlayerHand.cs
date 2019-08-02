using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

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

        // 착용 가능한 도구들. Hand-Tool에 있는 것들 전부
        public string[] EquipableTools;
        // 현재 착용 중인 도구


        private void Start()
        {
            int count = transform.GetChild(0).childCount;

            EquipableTools = new string[count];

            for (int i = 0; i < count; i++) 
            {
                Debug.Log(transform.GetChild(0).GetChild(i).transform.name);
                EquipableTools.SetValue(transform.GetChild(0).GetChild(i).transform.name, i);
            }
        }

        private void Update()
        {
            transform.position = HandPosition.transform.position;
            transform.rotation = HandPosition.transform.rotation;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {

            }
        }

        void swapTools(int n)
        {

        }
    }
}
