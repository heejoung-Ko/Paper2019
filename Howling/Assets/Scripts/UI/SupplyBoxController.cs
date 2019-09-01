using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Howling;

public class SupplyBoxController : MonoBehaviour
{
    [SerializeField]
    Transform slots;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(GameObject go)
    {
        Item[] items = go.GetComponent<SupplyBox>().items;

        for (int i = 0; i < slots.childCount; i++) 
        {
            slots.GetChild(i).GetComponent<Slot>().AddItem(items[i], 1);
        }
    }
}
