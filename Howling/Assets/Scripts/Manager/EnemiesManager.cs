using System;
using System.Collections;
using UnityEngine;

public enum EnemyType
{
    RABBIT, FOX, DEER, BOAR, BEAR
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
    [SerializeField] public static float randomNum = 20f;

    public WolfAgent wolfAgent;
    public GameObject playerTarget;

    public string[] enemiesName;
    public int[] enemiesNum;

    [HideInInspector] public bool isNight;
    public StatusController statusController;
    public EffectCameraController effectCameraController;
    
    [HideInInspector] public int bearOldDay = 0;
    static int bearDayCycle = 1;
    [HideInInspector] public bool isBearTraceAtNight;
    [HideInInspector] public float stopTimeTraceAtNight = 15f;

    private void Awake()
    {
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < enemiesNum[i]; ++j)
            {
                GameObject new_enemy = ObjectPool.Instance.PopFromPool(enemiesName[i]);

                int randomSpawn = UnityEngine.Random.Range(0, enemies[i].enemiesSpawn.Length);
                Transform spawn = enemies[i].enemiesSpawn[randomSpawn] as Transform;
                Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-randomNum, randomNum), 0, UnityEngine.Random.Range(-randomNum, randomNum));
                Quaternion randomRot = Quaternion.Euler(spawn.rotation.x, spawn.rotation.y + UnityEngine.Random.Range(0, 360), spawn.rotation.z);
                new_enemy.gameObject.SetActive(true);
                new_enemy.transform.position = spawn.position + randomPos;
                new_enemy.transform.rotation = randomRot;
            }
        }
    }

    private void Start()
    {
        wolfAgent = FindObjectOfType<WolfAgent>();
    }

    public void AtkReward(int atk)
    {
        if (wolfAgent == null)
        {
            Debug.Log("EnemiesManager - Wolf Agent is null.");
            wolfAgent = FindObjectOfType<WolfAgent>();
            return;
        }
        wolfAgent.EnemyAtkReward(atk);
    }

    public void DieReward()
    {
        if (wolfAgent == null)
        {
            Debug.Log("EnemiesManager - Wolf Agent is null.");
            wolfAgent = FindObjectOfType<WolfAgent>();
            return;
        }
        wolfAgent.EnemyDieReward();
    }

    internal void Die(GameObject enemy)
    {
        StartCoroutine(DestroyEnemy(enemy));
    }

    public void SetTraceAtNightByDay(int day)
    {
        if (bearOldDay + bearDayCycle == day)
        {
            bearOldDay = day;
            Debug.Log("isBearTraceAtNight true!");
            isBearTraceAtNight = true;
        }
        StartCoroutine(SetOffTraceAtNight());
    }

    public IEnumerator SetOffTraceAtNight()
    {
        yield return new WaitForSeconds(stopTimeTraceAtNight);
        Debug.Log("isTraceAtNight false!");
        isBearTraceAtNight = false;
    }

    private IEnumerator DestroyEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(destroyTime);

        for (int i = 0; i < dropItems.Length; ++i)
            Instantiate(dropItems[i], enemy.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

        ObjectPool.Instance.PushToPool(enemy.name, enemy);
        //Debug.Log(enemy.gameObject.name + " - SetActive(false)");
        StartCoroutine(RespawnEnemy(enemy));
    }

    private IEnumerator RespawnEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(respawnTime);

        Debug.Log(enemy.name);

        GameObject new_enemy = ObjectPool.Instance.PopFromPool(enemy.name);

        int enemyId = 0;
        if (enemy.name.Equals("Rabbit")) enemyId = 0;
        else if (enemy.name.Equals("Fox")) enemyId = 1;
        else if (enemy.name.Equals("Deer")) enemyId = 2;
        else if (enemy.name.Equals("Boar")) enemyId = 3;
        else if (enemy.name.Equals("Bear")) enemyId = 4;

        int randomSpawn = UnityEngine.Random.Range(0, enemies[enemyId].enemiesSpawn.Length);
        Transform spawn = enemies[enemyId].enemiesSpawn[randomSpawn] as Transform;
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-randomNum, randomNum), 0, UnityEngine.Random.Range(-randomNum, randomNum));
        Quaternion randomRot = Quaternion.Euler(spawn.rotation.x, spawn.rotation.y + UnityEngine.Random.Range(0, 360), spawn.rotation.z);
        new_enemy.gameObject.SetActive(true);
        new_enemy.transform.position = spawn.position + randomPos;
        new_enemy.transform.rotation = randomRot;
    }
}