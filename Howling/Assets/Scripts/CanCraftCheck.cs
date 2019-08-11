using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanCraftCheck : MonoBehaviour
{
    public bool canCraft;
    
    [SerializeField]
    int mapLayer;
    
    [SerializeField]
    GameObject can;
    
    [SerializeField]
    GameObject cant;
    
    List<Collider> colliderList = new List<Collider>();
    
    // Update is called once per frame
    void Update()
    {
        if (colliderList.Count == 0)
            canCraft = true;
        else
            canCraft = false;

        if (canCraft)
        {
            can.SetActive(true);
            cant.SetActive(false);
        }
        else
        {
            can.SetActive(false);
            cant.SetActive(true);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != mapLayer)
            colliderList.Add(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != mapLayer)
            colliderList.Remove(other);
    }
    
    public bool checkCraft()
    {
        return canCraft;
    }
}