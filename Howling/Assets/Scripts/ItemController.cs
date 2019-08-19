using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static float maxTime = 180f;

    [SerializeField]
    private float Durability = 10f;

    void Start()
    {
        Destroy(gameObject, maxTime);
    }

    public void setDurability(float n)
    {
        Durability = n;
    }

    public float getDurability()
    {
        return Durability;
    }
}
