using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnInSea : MonoBehaviour
{
    [SerializeField]
    GameObject spawnPoint;

    [SerializeField]
    GameObject sun;

    [SerializeField]
    Item[] spawnItem = new Item[3];

    [SerializeField]
    LayerMask itemLayer;

    Vector3 boxSize = new Vector3(20, 5, 35);

    float finalSpawnTime = 0f;

    private void Start()
    {
        respawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, itemLayer);

        foreach (Collider c in colliders)
        {
           c.transform.position = new Vector3(c.transform.position.x - 5 * Time.deltaTime, c.transform.position.y, c.transform.position.z);
        }

        float time = sun.GetComponent<DayNightCycle>().percentageOfDay;
        time *= 2 * 12;

        if (finalSpawnTime + 24 < time)
        {
            respawnItem();
            finalSpawnTime = time;
        }
    }

    void respawnItem()
    {
        for (int i = 0; i < 5; i++)
        {
            int itemNum = Random.RandomRange(0, 3);

            GameObject go = Instantiate(spawnItem[itemNum].ItemPrefab);
            float x = Random.Range(2, 5);
            float z = Random.Range(-boxSize.z, boxSize.z);
            go.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
            // Debug.Log(go.transform.position);
            //go.transform.position = transform.position;
        }
    }
}
