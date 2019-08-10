using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Howling;
using UnityEngine.EventSystems;

public class BoxController : MonoBehaviour
{
    [SerializeField]
    GameObject slots;
    [SerializeField]
    GameObject slotsBase;

    [SerializeField]
    GameObject inventory;

    [SerializeField]
    GameObject dragSlot;

    Transform[] slotList;

    public void BoxStart()
    {
        inventory.GetComponent<Inventory>().setColor(1);

        int cnt = inventory.transform.GetChild(0).GetChild(0).GetChildCount();
        for (int i = 0; i < cnt; i++)
        {
            GameObject baseGo = inventory.transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
            GameObject go = Instantiate(baseGo);

            go.transform.parent = slots.transform;

            go.transform.position = baseGo.transform.position;
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        cnt = slotsBase.transform.GetChildCount();

        for (int i = 0; i<cnt; i++)
        {
            GameObject slotBaseList = slotsBase.transform.GetChild(i).GetChild(0).gameObject;
            int c = slotBaseList.transform.GetChildCount();
            for(int j = 0; j<c; j++)
            {
                GameObject baseGo = slotBaseList.transform.GetChild(j).gameObject;
                GameObject go = Instantiate(baseGo);

                go.transform.parent = slots.transform;

                go.transform.position = baseGo.transform.position;
                go.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        slotList = slots.GetComponentsInChildren<Transform>();
    }

    public void BoxEnd()
    {
        UpdateInventory();

        inventory.GetComponent<Inventory>().setColor(200f/255f);

        if (slotList != null)
        {
            for (int i = 0; i < slotList.Length; i++)
            {
                if (slotList[i] != slots.transform)
                    Destroy(slotList[i].gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            inventory.GetComponent<Inventory>().SwapItem();
    }

    void UpdateInventory()
    {
        if (inventory == null)
            Debug.Log("인벤토리 어디감?");

        Transform[] invenList = inventory.transform.GetChild(0).GetComponentsInChildren<Transform>();

        if (invenList != null)
        {
            for (int i = 0; i < invenList.Length; i++)
            {
                invenList[i] = slotList[i];
            }
        }
    }
}
