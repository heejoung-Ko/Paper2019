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

    [SerializeField]
    GameObject wave;

    private float waveHeight;
    private float waveFrequency;
    private float waveLength;

    private Vector3 waveOriginPosition;

    private void Start()
    {
        respawnItem();

        waveHeight = wave.GetComponent<LowPolyWater.LowPolyWater>().waveHeight;
        waveFrequency = wave.GetComponent<LowPolyWater.LowPolyWater>().waveFrequency;
        waveLength = wave.GetComponent<LowPolyWater.LowPolyWater>().waveLength;

        waveOriginPosition = wave.GetComponent<LowPolyWater.LowPolyWater>().waveOriginPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, itemLayer);

        float time = sun.GetComponent<DayNightCycle>().percentageOfDay;
        time *= 2 * 12;

        Debug.Log(Mathf.Sin(time) * 0.02f);

        foreach (Collider c in colliders)
        {
            float distance = Vector3.Distance(c.transform.position, waveOriginPosition);
            distance = (distance % waveLength) / waveLength;

            float y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 1.0f * waveFrequency + (Mathf.PI * 2.0f * distance)) * 0.5f;

            distance = Vector3.Distance(transform.position, waveOriginPosition);

            y = y / distance * 10 + transform.position.y;

            c.transform.position = new Vector3(c.transform.position.x - 2 * Time.deltaTime / distance * 30, y, c.transform.position.z);
        }


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
