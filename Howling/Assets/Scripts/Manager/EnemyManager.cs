using System;
using UnityEngine;

namespace Howling
{
    [Serializable]
    public class EnemyManager
    {
        public Transform m_SpawnPoint;
        [HideInInspector] public int m_EnemyNumber;
        [HideInInspector] public GameObject m_Instance;

        ////////////////////////////////////////////////////
        // 필요한 스크립트 변수 추가
        private Enemy m_Enemy;

        ////////////////////////////////////////////////////

        public void Setup()
        {
            m_Enemy = m_Instance.GetComponent<Enemy>();
            m_Enemy.m_EnemyNumber = m_EnemyNumber;
        }

        public void DisableControl()
        {
            m_Enemy.enabled = false;
        }

        public void EnableControl()
        {
            m_Enemy.enabled = true;
        }

        public void Reset()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive(false);
            m_Instance.SetActive(true);
        }
    }
}
