using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Howling
{
    public class GameManager : MonoBehaviour
    {
        public float m_StartDelay = 3f;
        public float m_EndDelay = 3f;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;

        public GameObject m_PlayerPrefab;
        public GameObject m_EnemyPrefab;

        public PLAYERManager m_Player;
        public EnemyManager[] m_Enemys;

        private static float m_RandomNumber = 10f;

        // Start is called before the first frame update
        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnPlayer();
            SpawnAllEnemy();

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
                m_Enemys[i].m_Instance = Instantiate(m_EnemyPrefab, m_Enemys[i].m_SpawnPoint.position + randomPos, m_Enemys[i].m_SpawnPoint.rotation) as GameObject;
                m_Enemys[i].m_EnemyNumber = i + 1;
                m_Enemys[i].Setup();
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
            Debug.Log("게임 스탙으~~`");
            ResetPlayer();
            ResetAllEnemys();
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
            Debug.Log("게임 끝~~~~~`");
            yield return m_EndWait;
        }


        private bool PlayerLeft()
        {
            Debug.Log("플레이어: " + m_Player.m_Instance.activeSelf);
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
                m_Enemys[i].Reset();
            }
        }

        private void EnableControl()
        {
            m_Player.EnableControl();
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                m_Enemys[i].EnableControl();
            }
        }

        private void DisableControl()
        {
            m_Player.DisableControl();
            for (int i = 0; i < m_Enemys.Length; i++)
            {
                m_Enemys[i].DisableControl();
            }
        }
    }
}