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

        private Slot[] slots;

        void Awake()
        {
            slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        }

        //void Update()
        //{
        //    TryOpenInventory();
        //}

        //private void TryOpenInventory()
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        inventoryActivated = !inventoryActivated;

        //        if (inventoryActivated)
        //            OpenInventory();
        //        else
        //            CloseInventory();
        //    }
        //}

        //private void OpenInventory()
        //{
        //    go_InventoryBase.SetActive(true);
        //}

        //private void CloseInventory()
        //{
        //    go_InventoryBase.SetActive(false);
        //}

        public void AcquireItem(Item acquireItem, int cnt = 1)
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