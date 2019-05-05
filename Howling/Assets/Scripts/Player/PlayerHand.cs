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
        //public float workSpeed;   // 작업 속도
        public float attackDelay;   // 공격 딜레이
        public float attackActiveDelay;  // 공격 활성화 시점.
        public float attackCancleDelay;  // 공격 비활성화 시점.

        //public Animator anim; // 애니메이션.
    }
}
