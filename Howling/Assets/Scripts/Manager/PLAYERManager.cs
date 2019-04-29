using System;
using UnityEngine;

namespace Howling
{
    [Serializable]
    public class PLAYERManager
    {
        public Transform m_SpawnPoint;
        [HideInInspector] public GameObject m_Instance;

        ////////////////////////////////////////////////////
        // 필요한 스크립트 변수 추가
        private PlayerMoveScript m_Movement;
        ////////////////////////////////////////////////////

        public void Setup()
        {
            m_Movement = m_Instance.GetComponent<PlayerMoveScript>();
        }

        public void DisableControl()
        {
            m_Movement.enabled = false;
        }

        public void EnableControl()
        {
            m_Movement.enabled = true;
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