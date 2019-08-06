using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestEnemyManager : MonoBehaviour
{
    [HideInInspector] public GameObject instance;
    public bool herbivoreRespawn;
    public bool carnivoreRespawn;
    public GameObject herbivorePrefab;
    public GameObject carnivorePrefab;
    public Transform pivotTransform;
    public Transform herbivoreSpawn;
    public Transform carnivoreSpawn;

    private MLTestEnemy herbivoreEnemy;
    private MLTestEnemy carnivoreEnemy;
    private float herbivoreRespawnTime;
    private float carnivoreRespawnTime;

    public void Setup()
    {
        herbivoreEnemy = instance.GetComponent<MLTestEnemy>();
        carnivoreEnemy = instance.GetComponent<MLTestEnemy>();
    }

    void Update()
    {
        if (herbivoreRespawn) herbivoreRespawnTime += 1;
        if (carnivoreRespawn) carnivoreRespawnTime += 1;
        if (herbivoreRespawnTime > 200)
        {
            herbivoreRespawnTime = 0;
            Vector3 randomPos = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));
            var position = randomPos + pivotTransform.position;
            herbivoreEnemy.instance = Instantiate(herbivorePrefab, position + randomPos, herbivoreSpawn.rotation) as GameObject;
        }
        if (carnivoreRespawnTime > 200)
        {
            carnivoreRespawnTime = 0;
            Vector3 randomPos = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));
            var position = randomPos + pivotTransform.position;
            carnivoreEnemy.instance = Instantiate(carnivorePrefab, position + randomPos, carnivoreSpawn.rotation) as GameObject;
        }
    }
}
