using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Vuforia;
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

    [SerializeField] private Item[] items;

    [SerializeField] private GameObject boxSlotsParent_line1;
    [SerializeField] private GameObject boxSlotsParent_line2;
    [SerializeField] private GameObject boxSlotsParent_line3;
    [SerializeField] private GameObject boxSlotsParent_line4;
    private Slot[] boxSlots_line1;
    private Slot[] boxSlots_line2;
    private Slot[] boxSlots_line3;
    private Slot[] boxSlots_line4;

    public Slot[] GetBoxSlot(int line)
    {
        switch(line) {
            case 1:
                return boxSlots_line1;
            case 2:
                return boxSlots_line2;
            case 3:
                return boxSlots_line3;
            case 4:
                return boxSlots_line4;
            default:
                return boxSlots_line4;
        }
    }
    //private Slot[][] boxSlots;
    //public Slot[][] GetBoxSlots() {
    //    return boxSlots;
    //}

    private void Start()
    {
        boxSlots_line1 = boxSlotsParent_line1.GetComponentsInChildren<Slot>();
        boxSlots_line2 = boxSlotsParent_line2.GetComponentsInChildren<Slot>();
        boxSlots_line3 = boxSlotsParent_line3.GetComponentsInChildren<Slot>();
        boxSlots_line4 = boxSlotsParent_line4.GetComponentsInChildren<Slot>();
    }

    public void LoadToBoxLine(int line, int _arrayNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i].ItemName == _itemName)
            {
                switch(line)
                {
                    case 1:
                        boxSlots_line1[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 2:
                        boxSlots_line1[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 3:
                        boxSlots_line1[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 4:
                        boxSlots_line1[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                }
            }
        }
    }

    public void BoxStart()
    {
        inventory.GetComponent<Inventory>().setColor(1);

        int cnt = inventory.transform.GetChild(0).GetChild(0).childCount;
        for (int i = 0; i < cnt; i++)
        {
            GameObject baseGo = inventory.transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
            GameObject go = Instantiate(baseGo);

            go.transform.parent = slots.transform;

            go.transform.position = baseGo.transform.position;
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        cnt = slotsBase.transform.childCount;

        for (int i = 0; i<cnt; i++)
        {
            GameObject slotBaseList = slotsBase.transform.GetChild(i).GetChild(0).gameObject;
            int c = slotBaseList.transform.childCount;
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
