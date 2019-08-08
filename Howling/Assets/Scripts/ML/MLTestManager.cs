using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLTestManager : MonoBehaviour
{
    //[HideInInspector] public GameObject instance;
    public bool herbivoreRespawn;
    public bool carnivoreRespawn;
    public GameObject herbivorePrefab;
    public GameObject carnivorePrefab;
    public GameObject meatPrefab;
    public GameObject feedPrefab;
    public Transform pivotTransform;
    public Transform herbivoreSpawn;
    public Transform carnivoreSpawn;
    public Transform playerPos;

    private float herbivoreRespawnTime;
    private float carnivoreRespawnTime;

    private float meatRespawnTime;
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

        // meat 
        meatRespawnTime += 1;
        if (meatRespawnTime > 1000)
        {
            meatRespawnTime = 0;
            //Invoke("DropMeat", 5f);
            Invoke("DropFeed", 5f);
        }
    }

    void DropMeat()
    {
        Debug.Log("Manager - Drop Meat!");
        Vector3 randomPos = new Vector3(Random.Range(minRange, maxRange), 0.5f, Random.Range(minRange, maxRange));
        var position = randomPos + pivotTransform.position;
        Instantiate(meatPrefab, position, Quaternion.identity);
    }

    void DropFeed()
    {
        Debug.Log("Manager - Drop Feed!");
        Vector3 randomPos = new Vector3(Random.Range(minRange / 3, maxRange / 3), 0.5f, Random.Range(minRange / 3, maxRange / 3));
        var position = randomPos + playerPos.position;
        Instantiate(feedPrefab, position, Quaternion.identity);
    }
}
