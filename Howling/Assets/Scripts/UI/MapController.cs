using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject player;
    public GameObject playerPointer;

    int mapWidth = 350;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(playerPointer.transform.localPosition);   
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pointerPosition = new Vector3(player.transform.position.x / 350 * 250, player.transform.position.z / 350 * 250);
        playerPointer.transform.localPosition = pointerPosition;

        Quaternion pointerRotation = new Quaternion(0, 0, -player.transform.rotation.y, player.transform.rotation.w);
        playerPointer.transform.rotation = pointerRotation;

    }
}
