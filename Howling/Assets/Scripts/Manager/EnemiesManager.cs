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
        public GameObject enemiesPrefab;            // 종류
        public int maxNum;                          // Prefab 종류 순서대로 소환 되는 수
        //public Transform[] enemiesSpawn;            // 종류별로 스폰 갯수, 위치
        [HideInInspector] public ArrayList enemiesSpawn;            // 종류별로 스폰 갯수, 위치
        [HideInInspector] public int currentNum;
        [HideInInspector] public Enemy enemy;
    }
    public EnemiesDB[] enemies;
    public GameObject[] dropItems;
    public Transform[] spawnPoints;
    [SerializeField] public static float randomNum = 10f;

    public WolfAgent wolfAgent;

    private void Start()
    {
        wolfAgent = FindObjectOfType<WolfAgent>();
        InitialSpawnPoints();
        SpawnAllEnemies();
    }

    private void InitialSpawnPoints()
    {
        for (int k = 0; k < enemies.Length; ++k)
        {
            enemies[k].enemiesSpawn = new ArrayList();
            if (enemies[k].type == EnemyType.RABBIT)
            {
                for (int i = 0; i < (int)(spawnPoints.Length * 0.8); ++i)
                    enemies[k].enemiesSpawn.Add(spawnPoints[i]);
            }
            else if (enemies[k].type == EnemyType.FOX)
            {
                for (int i = (int)(spawnPoints.Length * 0.2); i < (int)(spawnPoints.Length * 0.6); ++i)
                    enemies[k].enemiesSpawn.Add(spawnPoints[i]);
            }
            else if (enemies[k].type == EnemyType.DEAR)
            {
                for (int i = (int)(spawnPoints.Length * 0.6); i < (int)(spawnPoints.Length * 0.8); ++i)
                    enemies[k].enemiesSpawn.Add(spawnPoints[i]);
            }
            else if (enemies[k].type == EnemyType.BOAR)
            {
                for (int i = (int)(spawnPoints.Length * 0.8); i < (int)(spawnPoints.Length * 0.9); ++i)
                    enemies[k].enemiesSpawn.Add(spawnPoints[i]);
            }
            else if (enemies[k].type == EnemyType.BEAR)
            {
                for (int i = (int)(spawnPoints.Length * 0.9); i < spawnPoints.Length; ++i)
                    enemies[k].enemiesSpawn.Add(spawnPoints[i]);
            }
        }
    }

    private void SpawnAllEnemies()
    {
        for (int k = 0; k < enemies.Length; ++k)
        {
            for (int i = 0; i < enemies[k].maxNum; ++i)
            {
                SpawnEnemy(k);
            }
        }
    }

    public void SpawnEnemy(int k)
    {
        int randomSpawn = Random.Range(0, enemies[k].enemiesSpawn.Count);
        Transform spawn = enemies[k].enemiesSpawn[randomSpawn] as Transform;
        Vector3 randomPos = new Vector3(Random.Range(-randomNum, randomNum), 0, Random.Range(-randomNum, randomNum));
        Quaternion randomRot = Quaternion.Euler(spawn.rotation.x, spawn.rotation.y + Random.Range(0, 360), spawn.rotation.z);
        GameObject instance = Instantiate(enemies[k].enemiesPrefab, spawn.position + randomPos, randomRot) as GameObject;
        enemies[k].enemy = instance.GetComponent<Enemy>();
        enemies[k].enemy.type = enemies[k].type;
        enemies[k].currentNum += 1;
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
        //if (wolfAgent == null) wolfAgent = FindObjectOfType<WolfAgent>();
        if (wolfAgent == null)
        {
            Debug.Log("EnemiesManager - Wolf Agent is null.");
            return;
        }
        wolfAgent.EnemyDieReward();
    }

    public void Die(Enemy enemy)
    {
        enemies[(int)enemy.type].currentNum -= 1;
        StartCoroutine(DestroyEnemy(enemy));
        StartCoroutine(AddEnemy(enemy));
    }

    IEnumerator DestroyEnemy(Enemy enemy)
    {
        yield return new WaitForSeconds(destroyTime);
        //Debug.Log("아이템 뿌린당!!!");
        for(int i = 0; i < dropItems.Length; ++i)
            Instantiate(dropItems[i], enemy.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        Destroy(enemy.gameObject);
    }

    IEnumerator AddEnemy(Enemy enemy)
    {
        yield return new WaitForSeconds(respawnTime);
        for (int k = 0; k < enemies.Length; ++k)
        {
            if (enemies[k].currentNum >= enemies[k].maxNum) continue;
            SpawnEnemy(k);
        }
    }
}