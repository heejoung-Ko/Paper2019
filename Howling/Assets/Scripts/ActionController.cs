using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;                    // 습득 가능한 최대 거리

    private bool pickUpActivated = false;   // 습득이 가능한지 활성화

    private RaycastHit hitInfo;             // 충돌체 정보 저장

    // 아이템 레이어에만 반응하도록 레이어 마스크 설정
    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField]
    private Text actionText;

    // Update is called once per frame
    void Update()
    {
        CheckItem();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
        }
    }

    private void CanPickUp()
    {
        if (pickUpActivated)
        {
            if (hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUP>().item.ItemName + "획득");
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
            }
        }
    }

    private void CheckItem()
    {
        Vector3 forward = new Vector3(transform.forward.x, transform.parent.forward.y, 0);
        if (Physics.Raycast(transform.parent.position, forward, out hitInfo, range, layerMask))
        {
            
            if (hitInfo.transform.tag == "item")
            {
                ItemInfoAppear();
            }
        }
        else
            ItemInfoDisappear();
    }

    private void ItemInfoAppear()
    {
        Debug.Log("ItemInfoAppear");
        pickUpActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUP>().item.ItemName + "획득" + "<color=yellow>" + "(E)키" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickUpActivated = false;
        actionText.gameObject.SetActive(false);
    }
}