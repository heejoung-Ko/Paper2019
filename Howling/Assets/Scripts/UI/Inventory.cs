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
                SwapSlot(0);
            }
           else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwapSlot(1);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwapSlot(2);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                SwapSlot(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SwapSlot(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SwapSlot(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SwapSlot(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SwapSlot(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SwapSlot(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SwapSlot(9);
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
                    if (Item.ItemType.Equipment == acquireItem.itemType && slots[i] == selectSlot)
                        SwapItem();
                    return;
                }
            }    
        }

        public void SwapSlot(int n)
        {
            if (selectSlot == slots[n]) ;
            else
            {
                selectSlot.SelectSlot(false);
                selectSlot = slots[n];
                selectSlot.SelectSlot(true);

                SwapItem();
            }
        }

        public void SwapItem()
        {
            if (selectSlot.item == null)
            {
                go_PlayerHand.GetComponent<PlayerHand>().swapTools(-1);
            }
            else if (selectSlot.item != null && selectSlot.item.itemType == Item.ItemType.Equipment)
            {
                go_PlayerHand.GetComponent<PlayerHand>().swapTools(selectSlot.item.weaponType);

            }
        }

        public int getItemNum(Item item)
        {
            int c = 0;
            for (int i = 0; i < slots.Length; i++) 
            {
                if (item == slots[i].item)
                    c+= slots[i].itemCount;
            }
            return c;
        }

        public void subItem(Item acquireItem, int cnt)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.ItemName == acquireItem.ItemName)
                    {
                        slots[i].SetSlotCount(-cnt);
                        return;
                    }
                }
            }
        }
    }
}