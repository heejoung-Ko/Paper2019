using Howling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeMaterial : MonoBehaviour
{
    public GameObject inventory;
    bool canMake = false;

    public void SetMaterial(Recipe recipe)
    {
        UnsetMaterial();

        GameObject go;
        if (recipe.Material1 != null)
        {
            go = transform.GetChild(0).gameObject;
            go.SetActive(true);
            go.transform.GetChild(0).GetComponent<Image>().sprite = recipe.Material1.ItemImage_64x64;

            int requestItemNum = recipe.Material1_num;
            int holdItemNum = inventory.GetComponent<Inventory>().getItemNum(recipe.Material1);

            go.transform.GetChild(1).GetComponent<Text>().text = "(" + holdItemNum + "/" + requestItemNum + ")";
            if (holdItemNum < requestItemNum)
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.red;
                canMake = false;
            }
            else
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                canMake = true;
            }
        }
        if (recipe.Material2 != null)
        {
            go = transform.GetChild(1).gameObject;
            go.SetActive(true);
            go.transform.GetChild(0).GetComponent<Image>().sprite = recipe.Material2.ItemImage_64x64;

            int requestItemNum = recipe.Material2_num;
            int holdItemNum = inventory.GetComponent<Inventory>().getItemNum(recipe.Material2);

            go.transform.GetChild(1).GetComponent<Text>().text = "(" + holdItemNum + "/" + requestItemNum + ")";
            if (holdItemNum < requestItemNum)
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.red;
                canMake = false;
            }
            else
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                canMake = true;
            }
        }
        if (recipe.Material3 != null)
        {
            go = transform.GetChild(2).gameObject;
            go.SetActive(true);
            go.transform.GetChild(0).GetComponent<Image>().sprite = recipe.Material3.ItemImage_64x64;

            int requestItemNum = recipe.Material3_num;
            int holdItemNum = inventory.GetComponent<Inventory>().getItemNum(recipe.Material3);

            go.transform.GetChild(1).GetComponent<Text>().text = "(" + holdItemNum + "/" + requestItemNum + ")";
            if (holdItemNum < requestItemNum)
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.red;
                canMake = false;
            }
            else
            {
                go.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                canMake = true;
            }
        }
    }

    public void UnsetMaterial()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        canMake = false;
    }

    public bool canMaking() { return canMake; }
}
