using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Howling;

[System.Serializable]
class Craft
{
    public Item item;
    public GameObject prefab;           // 실제 설치되는 프리펩
    public GameObject prefabPreview;    // 설치 프리뷰 프리펩

}

public class CraftController : MonoBehaviour
{
    [SerializeField]
    private Craft Box;
    [SerializeField]
    private Craft CampFire;
    [SerializeField]
    private Craft Fence;

    [SerializeField]
    private Craft nowCraft = null;

    [SerializeField]
    GameObject Inventory;

    bool isBox = false;
    bool isCampfire = false;
    bool isFence = false;

    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    [SerializeField]
    private GameObject go;

    // Update is called once per frame
    void Update()
    {
        CheckInventory();

        if (isBox)
        {
            RotateRL();
            ViewPreviewOther();
            if (Input.GetMouseButtonDown(1))
                CraftBuilding();
        }
        else if (isCampfire)
        {
            RotateRL();
            ViewPreviewCampfire();
            if (Input.GetMouseButtonDown(1))
                CraftBuilding();
        }
        else if (isFence)
        {
            RotateFB();
            ViewPreviewOther();
            if (Input.GetMouseButtonDown(1))
                CraftBuilding();
        }
    }

    void CheckInventory()
    {
        Item select = Inventory.GetComponent<Inventory>().getSelectItem();

        if (select == Box.item)
        {
            if (isBox)
                return;

            isBox = true;
            isCampfire = false;
            isFence = false;

            if (go != null)
                Destroy(go);
            nowCraft = Box;
            go = Instantiate(nowCraft.prefabPreview);
        }
        else if (select == CampFire.item)
        {
            if (isCampfire)
                return;

            isBox = false;
            isCampfire = true;
            isFence = false;

            if (go != null)
                Destroy(go);
            nowCraft = CampFire;
            go = Instantiate(nowCraft.prefabPreview);
        }
        else if(select == Fence.item)
        {
            if (isFence)
                return;

            isBox = false;
            isCampfire = false;
            isFence = true;

            if (go != null)
                Destroy(go);
            nowCraft = Fence;
            go = Instantiate(nowCraft.prefabPreview);
        }
        else
        {
            isBox = false;
            isCampfire = false;
            isFence = false;
            if (go != null)
                Destroy(go);
            
        }
    }

    void ViewPreviewOther()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 location = hitInfo.point;
                go.transform.position = location;
            }
        }
        else
        {
            go.transform.position = new Vector3(0f, 0f, 0f);
        }
    }

    void ViewPreviewCampfire()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 location = hitInfo.point;
                go.transform.position = new Vector3(location.x, location.y + 0.2f, location.z);
            }
        }
        else
        {
            go.transform.position = new Vector3(0f, 0f, 0f);
        }
    }

    void CraftBuilding()
    {
        if (go.transform.position == new Vector3(0f, 0f, 0f))
            return;

        if (!go.GetComponent<CanCraftCheck>().checkCraft())
            return;

        Vector3 position = go.transform.position;
        Quaternion rotation = go.transform.rotation;

        Destroy(go);
        GameObject newGo = Instantiate(nowCraft.prefab);
        newGo.transform.position = position;
        newGo.transform.rotation = rotation;

        Inventory.GetComponent<Inventory>().subSelecSlot();

        isBox = false;
        isCampfire = false;
        isFence = false;
    }

    void RotateRL()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput > 0.1)
        {
            go.transform.Rotate(Vector3.up, 15);
        }
        else if (wheelInput < -0.1)
        {
            go.transform.Rotate(Vector3.down, 15);
        }
    }

    void RotateUD()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput > 0.1)
        {
            go.transform.Rotate(Vector3.left, 15);
        }
        else if (wheelInput < -0.1)
        {
            go.transform.Rotate(Vector3.right, 15);
        }
    }

    void RotateFB()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput > 0.1)
        {
            go.transform.Rotate(Vector3.forward, 15);
        }
        else if (wheelInput < -0.1)
        {
            go.transform.Rotate(Vector3.back, 15);
        }
    }
}
