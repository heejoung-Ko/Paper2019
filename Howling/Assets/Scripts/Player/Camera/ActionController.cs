using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Howling
{
    public class ActionController : MonoBehaviour
    {
        [SerializeField]
        private float range;                    // 습득 가능한 최대 거리

        private bool pickUpActivated = false;   // 습득이 가능한지 활성화

        private RaycastHit hitInfo;             // 충돌체 정보 저장
        private RaycastHit hitInfoBox;

        // 아이템 레이어에만 반응하도록 레이어 마스크 설정
        [SerializeField]
        private LayerMask itemMask;
      
        // 필요한 컴포넌트
        [SerializeField]
        private Text actionText;

        [SerializeField]
        private Image img;

        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        TutorialController tutorialController = null;

        [SerializeField]
        private LayerMask boxMask;

        [SerializeField]
        public GameObject UIManager;

        private bool boxOpen = false;

        Ray ray = new Ray();

        private void Start()
        {
            img.gameObject.SetActive(true);
            tutorialController = FindObjectOfType<TutorialController>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckItem();
            TryAction();
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red);
        }

        private void TryAction()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (tutorialController.currentShow > 3)
                {
                    CheckItem();
                    CanPickUp();
                    CheckBoxOpen();
                }
            }
        }

        private void CheckBoxOpen()
        {
            if (boxOpen)
                OpenBox();
        }

        private void CanPickUp()
        {
            if (pickUpActivated)
            {
                tutorialController.isPlayerGetItem = true;

                if (hitInfo.transform != null)
                {
                    // Debug.Log(hitInfo.transform.GetComponent<ItemPickUP>().item.ItemName + " 획득");
                    inventory.AddItem(hitInfo.transform.GetComponent<ItemPickUP>().item);
                    Destroy(hitInfo.transform.gameObject);
                    ItemInfoDisappear();
                    // animator.SetTrigger("PickUp");
                }
            }
        }

        private void CheckItem()
        {
            ray.origin = transform.position;
            ray.direction = transform.forward;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, itemMask))
            {
                if (hitInfo.transform.tag == "item")
                {
                    ItemInfoAppear();
                }

                if(hitInfo.transform.tag == "resource")
                {
                    ObjectInfoAppear();
                }
            }
            else
                ItemInfoDisappear();
            if (Physics.Raycast(transform.position, transform.forward, out hitInfoBox, range, boxMask))
            {
                BoxAppear();
            }
            else
                BoxDisapper();
        }

        private void ObjectInfoAppear()
        {
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.name + "선택됨";
        }
        private void ItemInfoAppear()
        {
            pickUpActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<ItemPickUP>().item.ItemName + "획득" + "<color=yellow>" + "(E)키" + "</color>";
        }

        private void ItemInfoDisappear()
        {
            pickUpActivated = false;
            actionText.gameObject.SetActive(false);
        }

        private void BoxAppear()
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "상자 열기" + "<color=yellow>" + "(E)키" + "</color>";

            boxOpen = true;
        }

        private void BoxDisapper()
        {
            if(!pickUpActivated)
                actionText.gameObject.SetActive(false);

            boxOpen = false;
        }

        private void OpenBox()
        {
            UIManager.GetComponent<UIManagerController>().enterBox();
        }
    }
}