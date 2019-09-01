using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Howling;

public class SupplyBoxController : MonoBehaviour
{
    [SerializeField]
    Transform slots;

    GameObject supplyBox = null;

    [SerializeField]
    GameObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetItemAll();
    }

    public void SetItem(GameObject go)
    {
        supplyBox = go;
        Item[] items = supplyBox.GetComponent<SupplyBox>().items;

        for (int i = 0; i < slots.childCount; i++) 
        {
            slots.GetChild(i).GetComponent<Slot>().AddItem(items[i], 1);
        }
    }

    void GetItemAll()
    {
        for (int i = 0; i < slots.childCount; i++)
        {
            if (slots.GetChild(i).GetComponent<Slot>().item != null)
            {
                inventory.GetComponent<Inventory>().AddItem(slots.GetChild(i).GetComponent<Slot>().item);
                slots.GetChild(i).GetComponent<Slot>().SetSlotCount(-1);
            }
        }
    }

    public void ClosedBox()
    {
        ResetSlots();
        Destroy(supplyBox);
    }

    public void ResetSlots()
    {
        for (int i = 0; i < slots.childCount; i++)
        {
            if (slots.GetChild(i).GetComponent<Slot>().item != null)
            {
                slots.GetChild(i).GetComponent<Slot>().DropItem(supplyBox);
            }
        }
    }
}
