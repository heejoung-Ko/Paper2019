using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderMapCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " - UnderMap trigger enter, destroy!");
        Destroy(other.gameObject);
    }
}
