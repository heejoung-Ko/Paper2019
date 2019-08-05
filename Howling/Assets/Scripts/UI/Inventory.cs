using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Howling
{
    public class Inventory : MonoBehaviour
    {
        public static bool inventoryActivated = false;

        [SerializeField]
        private GameObject go_InventoryBase;
        [SerializeField]
        private GameObject go_SlotsParent;      // Grid Setting

        [SerializeField]
        private GameObject go_PlayerHand;

        private Slot[] slots;
        private Slot selectSlot;

        void Awake()
        {
            slots = go_SlotsParent.GetComponentsInChildren<Slot>();
            selectSlot = slots[0];
            selectSlot.SelectSlot(true);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (selectSlot == slots[0]) ;
                else
                {
                    selectSlot.SelectSlot(false);
                    selectSlot = slots[0];
                    selectSlot.SelectSlot(true);

                    if (selectSlot.item == null)
                    {
                        go_PlayerHand.GetComponent<PlayerHand>().swapTools(-1);
                    }
                    else if (selectSlot.item != null && selectSlot.item.itemType == Item.ItemType.Equipment)
                    {
                        go_PlayerHand.GetComponent<PlayerHand>().swapTools(selectSlot.item.weaponType);
          
                    }
                }
            }
           else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (selectSlot == slots[1]) ;
                else
                {
                    selectSlot.SelectSlot(false);
                    selectSlot = slots[1];
                    selectSlot.SelectSlot(true);

                    if (selectSlot.item == null)
                    {
                        go_PlayerHand.GetComponent<PlayerHand>().swapTools(-1);
                    }
                    else if (selectSlot.item != null && selectSlot.item.itemType == Item.ItemType.Equipment)
                    {
                        go_PlayerHand.GetComponent<PlayerHand>().swapTools(selectSlot.item.weaponType);

                    }
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[2];
                selectSlot.SelectSlot(true);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[3];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[4];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[5];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[6];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[7];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[8];
                selectSlot.SelectSlot(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[9];
                selectSlot.SelectSlot(true);
            }
        }

        public void AddItem(Item acquireItem, int cnt = 1)
        {
            if (Item.ItemType.Equipment != acquireItem.itemType)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item != null)
                    {
                        if (slots[i].item.ItemName == acquireItem.ItemName)
                        {
                            slots[i].SetSlotCount(cnt);
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(acquireItem, cnt);
                    return;
                }
            }
        }
    }
}