using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestEnemyManager : MonoBehaviour
{
    public Transform m_SpawnPoint;
    [HideInInspector] public GameObject m_Instance;

    private MLTestEnemy m_TestEnemy;

    public void Setup()
    {
        m_TestEnemy = m_Instance.GetComponent<MLTestEnemy>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
