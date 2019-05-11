using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Howling
{
    public class GameManager : MonoBehaviour
    {
        public float m_StartDelay = 0f;
        public float m_EndDelay = 3f;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;

        public GameObject m_PlayerPrefab;
        public GameObject m_EnemyPrefab;
        public GameObject[] m_TreePrefab;

        public PLAYERManager m_Player;
        public EnemyManager[] m_Enemys;
        public TreeManager[] m_Trees;

        private static float m_RandomNumber = 10f;
        public static float m_TreeRandom = 20f;

        public Transform m_EnemySpawn;
        public Transform m_TreeSpawn;

        public int m_NumTreesSP;

        // Start is called before the first frame update
        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnPlayer();
            SpawnAllEnemy();
            SpawnAllTree();

            StartCoroutine(GameLoop());
        }

        private void SpawnPlayer()
        {
            m_Player.m_Instance = Instantiate(m_PlayerPrefab, m_Player.m_SpawnPoint.position, m_Player.m_SpawnPoint.rotation) as GameObject;
            m_Player.Setup();
        }

        private void SpawnAllEnemy()
        {
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-m_RandomNumber, m_RandomNumber), 0, Random.Range(-m_RandomNumber, m_RandomNumber));
                m_Enemys[i].m_Instance = Instantiate(m_EnemyPrefab, m_EnemySpawn.position + randomPos, m_EnemySpawn.rotation) as GameObject;
                m_Enemys[i].m_EnemyNumber = i + 1;
                m_Enemys[i].Setup();
            }
        }

        private void SpawnAllTree()
        {
            for(int i = 0; i<m_Trees.Length; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-m_TreeRandom, m_TreeRandom), 0, Random.Range(-m_TreeRandom, m_TreeRandom));
                Quaternion randomRot = Quaternion.Euler(m_TreeSpawn.rotation.x, m_TreeSpawn.rotation.y + Random.Range(0, 360), m_TreeSpawn.rotation.z);
                Transform spawnPoint = m_TreeSpawn.GetChild(Random.Range(0, m_NumTreesSP));
                int index = (int)Random.Range(0, 3);
                Debug.Log(index);
                m_Trees[i].m_Instance = Instantiate(m_TreePrefab[index], spawnPoint.position + randomPos, randomRot) as GameObject;
                m_Trees[i].m_TreeNumber = i + 1;
                m_Trees[i].Setup();
            }
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(GameStarting());
            yield return StartCoroutine(GamePlaying());
            yield return StartCoroutine(GameEnding());

            StartCoroutine(GameLoop());

            // 게임이 끝나서 씬을 다시 로드 해야 한다면
            //SceneManager.LoadScene(0);
        }

        private IEnumerator GameStarting()
        { 
            ResetPlayer();
            ResetAllEnemys();
            ResetAllTrees();
            //DisableControl();

            yield return m_StartWait;
            //yield return null;
        }

        private IEnumerator GamePlaying()
        {
            EnableControl();

            // 종료 조건
            while (PlayerLeft())
            {
                yield return null;
            }
        }

        private IEnumerator GameEnding()
        {
            DisableControl();
            yield return m_EndWait;
        }


        private bool PlayerLeft()
        {
            if (m_Player.m_Instance.activeSelf)
                return true;
            return false;
        }

        private void ResetPlayer()
        {
            m_Player.Reset();
        }

        private void ResetAllEnemys()
        {
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                m_Enemys[i].Reset(m_EnemySpawn);
            }
        }

        private void ResetAllTrees()
        {
            for (int i = 0; i < m_Trees.Length; i++)
            {
                m_Trees[i].Reset(m_TreeSpawn);
            }
        }

        private void EnableControl()
        {
            m_Player.EnableControl();
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                m_Enemys[i].EnableControl();
            }
            for (int i = 0; i < m_Trees.Length; i++)
            {
                m_Trees[i].EnableControl();
            }
        }

        private void DisableControl()
        {
            m_Player.DisableControl();
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                m_Enemys[i].DisableControl();
            }
            for (int i = 0; i < m_Trees.Length; i++)
            {
                m_Trees[i].DisableControl();
            }
        }
    }
}