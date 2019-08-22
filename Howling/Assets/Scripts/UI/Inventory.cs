﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public Slot[] GetInvenSlots() { return slots; }
        public Item[] items;

//        [SerializeField] private Item[] items;

        [SerializeField]
        private GameObject itemEffectDB;

        [SerializeField]
        private GameObject player;

        [SerializeField]
        private Item Feed;

        [SerializeField]
        private Item CookedMeat;

        [SerializeField]
        private Item Torch;

        [HideInInspector] public bool isGameOver = false;

        void Start()
        {
            items = Resources.LoadAll<Item>("Item");

            slots = go_SlotsParent.GetComponentsInChildren<Slot>();

            selectSlot = slots[0];
            selectSlot.SelectSlot(true);
        }

        private void Update()
        {
            if (isGameOver) return;

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

            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectSlot.DropItem();
                SwapItem();
            }

            if (Input.GetMouseButtonDown(1))
            {
                useItem();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                deactivateMeat();
            }

            if(selectSlot != null && selectSlot.item == Torch)
            {
                float time = Time.deltaTime;
                useSelectItem(time/ 10);    // 100초 동안 사용
            }
        }

        public void LoadToInven(int _arrayNum, string _itemName, int _itemNum)
        {
            for (int i = 0; i < items.Length; ++i)
            {
                if(items[i].ItemName == _itemName)
                {
                    slots[_arrayNum].AddItem(items[i], _itemNum);
                }
            }
        }

        public bool AddItem(Item acquireItem, int cnt = 1, float dur = 10)
        {
            if (Item.ItemType.Equipment != acquireItem.itemType && Item.ItemType.ETC != acquireItem.itemType && Item.ItemType.Recycle != acquireItem.itemType)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    if (slots[i].item != null)
                    {
                        if (slots[i].item.ItemName == acquireItem.ItemName && slots[i].itemCount < slots[i].getItemMax())
                        {
                            slots[i].SetSlotCount(cnt);
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < slots.Length; ++i)
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(acquireItem, cnt, dur);
                    if (Item.ItemType.Equipment == acquireItem.itemType && slots[i] == selectSlot)
                        SwapItem();
                    return true;
                }
            }
            return false;
        }

        public bool CheckCanAddItem(Item acquireItem, int cnt = 1)
        {
            if (Item.ItemType.Equipment != acquireItem.itemType && Item.ItemType.ETC != acquireItem.itemType && Item.ItemType.Recycle != acquireItem.itemType)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item != null)
                    {
                        if (slots[i].item.ItemName == acquireItem.ItemName && slots[i].itemCount < slots[i].getItemMax())
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return true;
                }
            }
            return false;
        }

        public void SwapSlot(int n)
        {
            if (selectSlot == slots[n]) return;

            selectSlot.SelectSlot(false);
            selectSlot = slots[n];
            selectSlot.SelectSlot(true);

            SwapItem();
        }

        public void SwapItem()
        {
            if (selectSlot.item == null || selectSlot.item.itemType != Item.ItemType.Equipment)
            {
                go_PlayerHand.GetComponent<PlayerHand>().swapTools(-1, 1);
            }
            else
            {
                go_PlayerHand.GetComponent<PlayerHand>().swapTools(selectSlot.item.weaponType, selectSlot.item.weaponAtk);
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
            int c = cnt;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item == acquireItem)
                    {
                        if (cnt == 0)
                            return;
                        slots[i].SetSlotCount(-1);
                        cnt -= 1;

                        i--;
                    }
                }
            }
        }

        void useItem()
        {
            if (player.GetComponent<PlayerAtk>().isDrink()) return;
            if (selectSlot.item != null)
            {
                if (selectSlot.item.itemType == Item.ItemType.Used)
                {
                    itemEffectDB.GetComponent<ItemEffectDB>().UseItem(selectSlot.item);
                    selectSlot.SetSlotCount(-1);
                    player.GetComponent<PlayerAtk>().setDrink();
                    SwapItem();
                }
                else if (selectSlot.item.itemType == Item.ItemType.Recycle)
                {
                    if (selectSlot.Durability > 0)
                    {
                        itemEffectDB.GetComponent<ItemEffectDB>().UseItem(selectSlot.item);
                        player.GetComponent<PlayerAtk>().setDrink();
                        useSelectItem(selectSlot.DurabilityMaxAbmount / 3f);
                    }
                }
            }
        }

        void deactivateMeat()
        {
            if (selectSlot.item != null && selectSlot.item.ItemName == "손질되지 않은 고기")
            {
                selectSlot.SetSlotCount(-1);
                AddItem(Feed, 1);
                SwapItem();
            }
        }

        public void setColor(float alpha)
        {
            Image image = transform.GetChild(0).GetComponent<Image>();

            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        public Item getSelectItem()
        {
            return selectSlot.item;
        }

        public void subSelecSlot()
        {
            selectSlot.SetSlotCount(-1);
            SwapItem();
        }

        public void useWoodToCampfire()
        {
            if (selectSlot != null && selectSlot.item.ItemName == "나무")
            {
                selectSlot.SetSlotCount(-1);
            }
            SwapItem();
        }

        public void useMeatToCampfire()
        {
            if (selectSlot != null && selectSlot.item.ItemName == "손질된 고기")
            {
                selectSlot.SetSlotCount(-1);
                AddItem(CookedMeat, 1);
            }
            SwapItem();
        }

        public void useSelectItem(float n = 1)
        {
            if (selectSlot.UseTool(n))
                subSelecSlot();
        }

        public void FillGaugeRecycleItem()
        {
            selectSlot.Durability = selectSlot.DurabilityMaxAbmount;
            selectSlot.GaugeUpdate();
        }
    }
}