using System.Collections;
using UnityEngine;

public enum EnemyType
{
    RABBIT, FOX, DEAR, BOAR, BEAR
}

public class EnemiesManager : MonoBehaviour
{
    public float destroyTime = 2f;
    public float respawnTime = 5f;
    [System.Serializable]
    public struct EnemiesDB
    {
        public EnemyType type;
        //public GameObject enemiesPrefab;            // 종류
        //public int maxNum;                          // Prefab 종류 순서대로 소환 되는 수
        //public Transform[] enemiesSpawn;            // 종류별로 스폰 갯수, 위치
        public Transform[] enemiesSpawn;            // 종류별로 스폰 갯수, 위치
        //[HideInInspector] public int currentNum;
        //[HideInInspector] public Enemy enemy;
    }
    public EnemiesDB[] enemies;
    public GameObject[] dropItems;
    //public Transform[] spawnPoints;
    [SerializeField] public static float randomNum = 10f;

    public WolfAgent wolfAgent;

    private void Start()
    {
        wolfAgent = FindObjectOfType<WolfAgent>();
    }

    public void AtkReward(int atk)
    {
        if (wolfAgent == null)
        {
            Debug.Log("EnemiesManager - Wolf Agent is null.");
            return;
        }
        wolfAgent.EnemyAtkReward(atk);
    }

    public void DieReward()
    {
        if (wolfAgent == null)
        {
            Debug.Log("EnemiesManager - Wolf Agent is null.");
            return;
        }
        wolfAgent.EnemyDieReward();
    }

    public void Die(Enemy enemy)
    {
        StartCoroutine(DestroyEnemy(enemy));
    }

    IEnumerator DestroyEnemy(Enemy enemy)
    {
        yield return new WaitForSeconds(destroyTime);
        //Debug.Log("아이템 뿌린당!!!");
        for(int i = 0; i < dropItems.Length; ++i)
            Instantiate(dropItems[i], enemy.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        enemy.gameObject.SetActive(false);
        //Debug.Log(enemy.gameObject.name + " - SetActive(false)");
        StartCoroutine(RespawnEnemy(enemy));
    }

    IEnumerator RespawnEnemy(Enemy enemy)
    {
        yield return new WaitForSeconds(respawnTime);
        int k = (int)enemy.type;
        int randomSpawn = Random.Range(0, enemies[k].enemiesSpawn.Length);
        Transform spawn = enemies[k].enemiesSpawn[randomSpawn] as Transform;
        Vector3 randomPos = new Vector3(Random.Range(-randomNum, randomNum), 0, Random.Range(-randomNum, randomNum));
        Quaternion randomRot = Quaternion.Euler(spawn.rotation.x, spawn.rotation.y + Random.Range(0, 360), spawn.rotation.z);
        enemy.gameObject.SetActive(true);
        enemy.transform.position = spawn.position + randomPos;
        enemy.transform.rotation = randomRot;
        enemy.ResetForRespawn();
        //Debug.Log(enemy.gameObject.name + " - SetActive(true)");
    }
}