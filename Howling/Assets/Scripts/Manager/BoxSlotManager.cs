using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Howling;

public class BoxSlotManager : MonoBehaviour
{
    public GameObject BoxController;

    [SerializeField] private GameObject boxSlotsParent_line1;
    [SerializeField] private GameObject boxSlotsParent_line2;
    [SerializeField] private GameObject boxSlotsParent_line3;
    [SerializeField] private GameObject boxSlotsParent_line4;
    public Slot[] boxSlots_line1;
    public Slot[] boxSlots_line2;
    public Slot[] boxSlots_line3;
    public Slot[] boxSlots_line4;

    public Item[] items;

    private void Start()
    {
        items = Resources.LoadAll<Item>("Item");

        boxSlots_line1 = boxSlotsParent_line1.GetComponentsInChildren<Slot>();
        boxSlots_line2 = boxSlotsParent_line2.GetComponentsInChildren<Slot>();
        boxSlots_line3 = boxSlotsParent_line3.GetComponentsInChildren<Slot>();
        boxSlots_line4 = boxSlotsParent_line4.GetComponentsInChildren<Slot>();

        if (boxSlots_line1[0] != null)
        {
            Debug.Log("박스 슬롯 0번째" + boxSlots_line1[0].name);
        }
    }

    public Slot[] GetBoxSlot(int line)
    {
        switch (line)
        {
            case 1:
                if (boxSlots_line1 != null) return boxSlots_line1;
                else return null;
            case 2:
                if (boxSlots_line2 != null) return boxSlots_line2;
                else return null;
            case 3:
                if (boxSlots_line3 != null) return boxSlots_line3;
                else return null;
            case 4:
                if (boxSlots_line4 != null) return boxSlots_line4;
                else return null;
            default:
                return null;
        }
    }

    public void LoadToBoxLine(int line, int _arrayNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i].ItemName == _itemName)
            {
                switch (line)
                {
                    case 1:
                        boxSlots_line1[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 2:
                        boxSlots_line2[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 3:
                        boxSlots_line3[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                    case 4:
                        boxSlots_line4[_arrayNum].AddItem(items[i], _itemNum);
                        break;
                }
            }
        }
    }

}
