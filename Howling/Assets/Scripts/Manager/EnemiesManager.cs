using System;
using System.Collections;
using UnityEngine;



public class EnemiesManager : MonoBehaviour
{
    //[Header("")]
    [Serializable]
    public struct EnemiesDB
    {
        public GameObject enemiesPrefab;   // 종류
        public Transform[] enemiesSpawn;   // 종류별로 스폰 갯수, 위치
        public int maxNum;                 // Prefab 종류 순서대로 소환 되는 수
        int currentNum;
    }
    public EnemiesDB[] enemies;

    //public GameObject[] enemiesPrefab;  
    //public Transform[,] enemiesSpawn;   
    //public int[] enemiesMaxNum;         
    public int numTreesSP;

    private static float randomNum = 10f;

    private void Start()
    {
        SpawnAllEnemies();
    }

    private void SpawnAllEnemies()
    {
        // TODO: 종류별로 스폰포인트 사용 다르게 (갯수, 위치 다)
        //for (int k = 0; k < enemiesPrefab.Length; ++k)
        //{
        //    for (int j = 0; j < enemiesSpawn.GetLength(k); ++j)
        //    {
        //        for (int i = 0; i < enemiesMaxNum.Length; ++i)
        //        {
        //            Vector3 randomPos = new Vector3(Random.Range(-randomNum, randomNum), 0, Random.Range(-randomNum, randomNum));
        //            Instantiate(enemiesPrefab[k], enemiesSpawn[k][j].position + randomPos, enemiesSpawn.rotation) as GameObject;
        //            m_Enemys[i].m_EnemyNumber = i + 1;
        //        }
        //    }
        //}
    }

    //private void SpawnAllTree()
    //{
    //    for (int i = 0; i < m_Trees.Length; i++)
    //    {
    //        Vector3 randomPos = new Vector3(Random.Range(-m_TreeRandom, m_TreeRandom), 0, Random.Range(-m_TreeRandom, m_TreeRandom));
    //        Quaternion randomRot = Quaternion.Euler(m_TreeSpawn.rotation.x, m_TreeSpawn.rotation.y + Random.Range(0, 360), m_TreeSpawn.rotation.z);
    //        Transform spawnPoint = m_TreeSpawn.GetChild(Random.Range(0, numTreesSP));
    //        int index = (int)Random.Range(0, 3);
    //        Debug.Log(index);
    //        m_Trees[i].m_Instance = Instantiate(enemiesPrefab[index], spawnPoint.position + randomPos, randomRot) as GameObject;
    //        m_Trees[i].m_TreeNumber = i + 1;
    //        m_Trees[i].Setup();
    //    }
    //}
}