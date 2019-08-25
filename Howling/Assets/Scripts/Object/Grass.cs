using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject[] grassPrefabs;
    public float groundRange;
    public int num;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Length)]) as GameObject;
            obj.transform.position = new Vector3(Random.Range(-groundRange, groundRange), 0, Random.Range(-groundRange, groundRange));
            obj.transform.parent = transform;
        }
    }
}
