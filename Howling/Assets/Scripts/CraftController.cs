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
    GameObject Inventory;

    bool isBox = false;
    bool isCampfire = false;

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
            ViewPreviewBox();
            if (Input.GetMouseButtonDown(1))
                CraftBox();
        }
        else if (isCampfire)
        {
            ViewPreviewCampfire();
            if (Input.GetMouseButtonDown(1))
                CraftCampfire();
        }
    }

    void CheckInventory()
    {
        Item select = Inventory.GetComponent<Inventory>().getSelectItem();

        if (select == Box.item)
        {
            isBox = true;
            isCampfire = false;
            if(go != null)
                Destroy(go);
            go = Instantiate(Box.prefabPreview);
        }
        else if (select == CampFire.item)
        {
            isCampfire = true;
            isBox = false;
            if (go != null)
                Destroy(go);
            go = Instantiate(CampFire.prefabPreview);
        }
        else
        {
            isBox = false;
            isCampfire = false;
            if (go != null)
                Destroy(go);
        }
    }

    void ViewPreviewBox()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 location = hitInfo.point;
                go.transform.position = location;
            }
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
    }

    void CraftBox()
    {
        if (go.transform.position == new Vector3(0f, 0f, 0f))
            return;

        Vector3 position = go.transform.position;

        Destroy(go);
        GameObject newGo = Instantiate(Box.prefab);
        newGo.transform.position = position;
        Inventory.GetComponent<Inventory>().subSelecSlot();
    }

    void CraftCampfire()
    {
        if (go.transform.position == new Vector3(0f, 0f, 0f))
            return;

        Vector3 position = go.transform.position;

        Destroy(go);
        GameObject newGo = Instantiate(CampFire.prefab);
        newGo.transform.position = position;
        Inventory.GetComponent<Inventory>().subSelecSlot();
    }
}
