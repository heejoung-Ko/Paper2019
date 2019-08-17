using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static float maxTime = 180f;

    void Start()
    {
        Destroy(gameObject, maxTime);
    }
}
