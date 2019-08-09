using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestManager : MonoBehaviour
{
    public bool herbivoreRespawn;
    public bool carnivoreRespawn;
    public GameObject herbivorePrefab;
    public GameObject carnivorePrefab;
    public Transform pivotTransform;
    public Transform herbivoreSpawn;
    public Transform carnivoreSpawn;

    private float herbivoreRespawnTime;
    private float carnivoreRespawnTime;

    private float minRange = -3;
    private float maxRange = 3;

    void Update()
    {
        // enemy 
        herbivoreRespawnTime += 1;
        carnivoreRespawnTime += 1;
        if (herbivoreRespawnTime > 1500)
        {
            herbivoreRespawnTime = 0;
            Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
            var position = randomPos + pivotTransform.position;
            Instantiate(herbivorePrefab, position + randomPos, herbivoreSpawn.rotation);
        }
        if (carnivoreRespawnTime > 3000)
        {
            carnivoreRespawnTime = 0;
            Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
            var position = randomPos + pivotTransform.position;
            Instantiate(carnivorePrefab, position + randomPos, carnivoreSpawn.rotation);
        }
    }
}
