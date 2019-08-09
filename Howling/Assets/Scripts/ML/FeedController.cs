using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedController : MonoBehaviour
{
    public GameObject feedPrefab;
    private float meatRespawnTime;
    private bool meatRespawn;

    private void FixedUpdate()
    {
        // meat

        if (meatRespawn)
        {
            meatRespawnTime += 1;
            if (meatRespawnTime > 200)
            {
                meatRespawnTime = 0;
                meatRespawn = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("agent"))
        {
            Debug.Log("player - agent 닿음!");
            if (!meatRespawn)
            {
                Debug.Log("player - DropFeed!");
                DropFeed();
                meatRespawn = true;
            }
        }
    }

    void DropFeed()
    {
        Debug.Log("Player - Drop Feed!");
        Vector3 randomPos = new Vector3(Random.Range(-1, 1), 0.5f, Random.Range(-1, 1));
        var position = randomPos + transform.position;
        Instantiate(feedPrefab, position, Quaternion.identity);
    }
}
