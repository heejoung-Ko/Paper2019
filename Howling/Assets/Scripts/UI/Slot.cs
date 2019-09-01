using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Howling
{
    public class Slot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        static public int itemMaxCount = 5;

        public static bool isSlotClick = false;
        public static bool isSlotDrag = false;

        public Item item;
        public int itemCount;
        public Image itemImage;

        [SerializeField]
        private Text text_Count;
        [SerializeField]
        private GameObject go_CountImage;

        private ItemEffectDB itemEffectDB;
        private TutorialController tutorialController;
        public Transform itemDropSpawn;
        public float itemDropForce;

        [HideInInspector] public float Durability;     // 내구도
        [HideInInspector] public float DurabilityMaxAbmount = 10f;
        [SerializeField]
        private GameObject Gauge;
        [SerializeField]
        private Image GaugeBar;

        void Start()
        {
            itemEffectDB = FindObjectOfType<ItemEffectDB>();
            isSlotClick = false;
            isSlotDrag = false;
            Durability = 0f;
            // tutorialController = FindObjectOfType<TutorialController>();
        }

        private void SetColor(float alpha)
        {
            Color color = itemImage.color;
            color.a = alpha;
            itemImage.color = color;

            if (item != null)
            {
                if (item.itemType != Item.ItemType.Equipment && item.itemType != Item.ItemType.Recycle)
                {
                    if (alpha - float.Epsilon <= 0f) go_CountImage.SetActive(false);
                    else go_CountImage.SetActive(true);
                }
            }
        }

        public void AddItem(Item addItem, int cnt = 1, float dur = 10f)
        {
            item = addItem;
            itemCount = cnt;
            itemImage.sprite = item.ItemImage_32x32;

            if (item.itemType != Item.ItemType.Equipment && item.itemType != Item.ItemType.Recycle)
            {
                go_CountImage.SetActive(true);
                text_Count.text = itemCount.ToString();
                Gauge.SetActive(false);
                Durability = 0;
            }
            else
            {
                text_Count.text = "0";
                go_CountImage.SetActive(false);
                Gauge.SetActive(true);
                Durability = dur;
            }

            SetColor(1);
        }

        public void SetSlotCount(int cnt)
        {
            itemCount += cnt;
            text_Count.text = itemCount.ToString();

            if (itemCount <= 0)
                ClearSlot();
        }

        private void ClearSlot()
        {
            item = null;
            itemCount = 0;
            itemImage.sprite = null;
            SetColor(0);

            text_Count.text = "0";
            go_CountImage.SetActive(false);
            Gauge.SetActive(false);
            Durability = 0;
        }

        public void DropItem()
        {
            if (item != null)
            {
                if (item.itemType != Item.ItemType.ETC)
                {
                    Rigidbody itemInstance = Instantiate(item.ItemPrefab.GetComponent<Rigidbody>(), itemDropSpawn.position + itemDropSpawn.forward / 2, itemDropSpawn.rotation) as Rigidbody;

                    if (itemInstance != null)
                    {
                        itemInstance.velocity = itemDropForce * itemDropSpawn.forward;

                        itemInstance.GetComponent<ItemController>().setDurability(Durability);
                    }
                }
                SetSlotCount(-1);
            }
        }

        public void DropItem(GameObject go)
        {
            if (item != null)
            {
                Rigidbody itemInstance = Instantiate(item.ItemPrefab.GetComponent<Rigidbody>(), go.transform.position, go.transform.rotation) as Rigidbody;

                SetSlotCount(-1);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (DragSlot.instance == null) return;
            if (eventData.button == PointerEventData.InputButton.Left)
                isSlotClick = true;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (item != null)
                {
                    if (tutorialController.currentShow > 5)
                    {
                        itemEffectDB.UseItem(item);
                        if (item.itemType == Item.ItemType.Used)
                            SetSlotCount(-1);
                        tutorialController.isPlayerUseItem = true;
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                isSlotClick = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DragSlot.instance == null) return;
            if (eventData.button == PointerEventData.InputButton.Left && isSlotClick)
            {
                if (item != null)
                {
                    isSlotDrag = true;
                    DragSlot.instance.dragSlot = this;
                    DragSlot.instance.DragSetImage(itemImage);

                    DragSlot.instance.transform.position = eventData.position;
                    SetColor(0);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (DragSlot.instance == null) return;

            if (item != null)
                {
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragSlot.instance == null) return;

            isSlotDrag = false;

            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
            if (item != null)
                SetColor(1);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (DragSlot.instance == null) return;

            if (DragSlot.instance.dragSlot != null)
                ChangeSlot();
        }

        private void ChangeSlot()
        {
            Item tempItem = item;
            int tempItemCount = itemCount;

            AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

            if (tempItem != null)
                DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);
            else if (tempItem == DragSlot.instance.dragSlot.item && itemCount + DragSlot.instance.dragSlot.itemCount > itemMaxCount)
            {
                int sumCnt = itemCount + DragSlot.instance.dragSlot.itemCount;
                itemCount = itemMaxCount;
                DragSlot.instance.dragSlot.itemCount = sumCnt - itemMaxCount;
            }
            else
                DragSlot.instance.dragSlot.ClearSlot();
        }

        public void SelectSlot(bool select)
        {
            if (select)
                transform.Find("Select").gameObject.SetActive(true);
            else
                transform.Find("Select").gameObject.SetActive(false);
        }

        public int getItemMax() { return itemMaxCount; }

        public bool UseTool(float n = 1f)
        {
            if (item == null) return false;
            if (item.itemType != Item.ItemType.Equipment && item.itemType != Item.ItemType.Recycle) return false;

            Durability -= n;
            if (Durability <= 0)
            {
                if (item.itemType == Item.ItemType.Equipment)
                {
                    ClearSlot();
                    return true;
                }
                else if (item.itemType == Item.ItemType.Recycle)
                {
                    Durability = 0;
                }
            }
            GaugeUpdate();
            return false;
        }

        public void GaugeUpdate()
        {
            GaugeBar.fillAmount = (float)(Durability / 10);
        }
    }
}