using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBox : MonoBehaviour
{
    [SerializeField]
    BoxCollider collider;
    [SerializeField]
    BoxCollider triger;

    bool isGround;

    public static int spawnItemNum = 3;
    [SerializeField]
    Item[] spawnItems = new Item[spawnItemNum];

    public static float waveHeight = 0.3f;
    public static float waveFrequency = 0.3f;
    public static float waveLength = 0.75f;
    
    public static Vector3 waveOriginPosition = new Vector3(0f, 0f, 0f);

    public static int itemNum = 5;
    [SerializeField]
    Item[] items = new Item[itemNum];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemNum; i++)
        {
            int n = Random.RandomRange(0, spawnItemNum);
            Debug.Log(items.Length);
            items[i] = spawnItems[n];

            isGround = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < 25)
            return;
        float distance = Vector3.Distance(transform.position, waveOriginPosition);
        distance = (distance % waveLength) / waveLength;

        float y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 1.0f * waveFrequency + (Mathf.PI * 2.0f * distance)) * 0.5f;

        distance = Vector3.Distance(transform.position, waveOriginPosition);

        y = y / distance /2 + transform.position.y;

        float rotationX = Random.Range(-5f, 5f);
        float rotationZ = Random.Range(-5f, 5f);

        if (!isGround)
        {
            transform.position = new Vector3(transform.position.x - 2 * Time.deltaTime / distance * 30, y, transform.position.z);

            // Debug.Log(rotationX);
            transform.Rotate(Vector3.forward, rotationX);
            transform.Rotate(Vector3.right, rotationZ);
        }
        else
            transform.position = new Vector3(transform.position.x - 2 * Time.deltaTime / distance * 30, transform.position.y, transform.position.z);   
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("map"))
        {
            isGround = true;
            collider.enabled = true;
            triger.enabled = false;
            //transform.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
