using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Howling
{
    public class Slot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
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

        void Start()
        {
            itemEffectDB = FindObjectOfType<ItemEffectDB>();
            isSlotClick = false;
            isSlotDrag = false;
            tutorialController = FindObjectOfType<TutorialController>();
        }

        private void SetColor(float alpha)
        {
            Color color = itemImage.color;
            color.a = alpha;
            itemImage.color = color;

            if (item != null)
            {
                if (item.itemType != Item.ItemType.Equipment)
                {
                    if (alpha - float.Epsilon <= 0f) go_CountImage.SetActive(false);
                    else go_CountImage.SetActive(true);
                }
            }
        }

        public void AddItem(Item addItem, int cnt = 1)
        {
            item = addItem;
            itemCount = cnt;
            itemImage.sprite = item.ItemImage;

            if (item.itemType != Item.ItemType.Equipment)
            {
                go_CountImage.SetActive(true);
                text_Count.text = itemCount.ToString();
            }
            else
            {
                text_Count.text = "0";
                go_CountImage.SetActive(false);
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
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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
            if (eventData.button == PointerEventData.InputButton.Left)
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
            if (item != null)
                {
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isSlotDrag = false;

            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
            if (item != null)
                SetColor(1);
        }

        public void OnDrop(PointerEventData eventData)
        {
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
            else
                DragSlot.instance.dragSlot.ClearSlot();
        }
    }
}