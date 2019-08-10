using System.Collections;
using UnityEngine;

public enum EnemyType
{
    RABBIT, FOX, DEAR, BOAR, BEAR
}

public class EnemiesManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemiesDB
    {
        public EnemyType type;
        public GameObject enemiesPrefab;            // 종류
        public int maxNum;                          // Prefab 종류 순서대로 소환 되는 수
        public Transform[] enemiesSpawn;            // 종류별로 스폰 갯수, 위치
        [HideInInspector] public int currentNum;
        [HideInInspector] public Enemy enemy;
    }
    public EnemiesDB[] enemies;
    public GameObject dropItem;
    public static float randomNum = 10f;

    private void Start()
    {
        SpawnAllEnemies();
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
        int randomSpawn = Random.Range(0, enemies[k].enemiesSpawn.Length);
        Transform spawn = enemies[k].enemiesSpawn[randomSpawn];
        Vector3 randomPos = new Vector3(Random.Range(-randomNum, randomNum), 0, Random.Range(-randomNum, randomNum));
        Quaternion randomRot = Quaternion.Euler(spawn.rotation.x, spawn.rotation.y + Random.Range(0, 360), spawn.rotation.z);
        GameObject instance = Instantiate(enemies[k].enemiesPrefab, spawn.position + randomPos, randomRot) as GameObject;
        enemies[k].enemy = instance.GetComponent<Enemy>();
        enemies[k].enemy.type = enemies[k].type;
        enemies[k].currentNum += 1;
    }

    public void Die(Enemy enemy)
    {
        //Invoke("DropItem", 5f);
        StartCoroutine(DropItem(enemy));
        enemies[(int)enemy.type].currentNum -= 1;
        Destroy(enemy.gameObject, 8f);
        Invoke("AddEnemy", 5f);
    }

    IEnumerator DropItem(Enemy enemy)
    {
        yield return new WaitForSeconds(7f);
        Debug.Log("아이템 뿌린당!!!");
        Instantiate(dropItem, enemy.transform.position, Quaternion.identity);
    }

    public void AddEnemy()
    {
        for (int k = 0; k < enemies.Length; ++k)
        {
            if (enemies[k].currentNum >= enemies[k].maxNum) return;
            SpawnEnemy(k);
        }
    }
}