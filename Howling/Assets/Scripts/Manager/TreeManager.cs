using System;
using UnityEngine;

namespace Howling
{
    [Serializable]
    public class TreeManager
    {
        [HideInInspector] public int m_TreeNumber;
        [HideInInspector] public GameObject m_Instance;

        ////////////////////////////////////////////////////
        // 필요한 스크립트 변수 추가

        ////////////////////////////////////////////////////

        public void Setup()
        {
        }

        public void DisableControl() { 
        }

        public void EnableControl()
        {
        }

        public void Reset(Transform SpawnPoint)
        {
            m_Instance.transform.position = m_Instance.transform.position;
            m_Instance.transform.rotation = m_Instance.transform.rotation;

            m_Instance.SetActive(false);
            m_Instance.SetActive(true);
        }
    }
}
