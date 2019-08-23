using Howling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Make : MonoBehaviour
{
    public Inventory inventory;
    public GameObject recipeList;
    public GameObject recipeMaterial;
    public GameObject making;

    void Update()
    {
        GameObject selectRecipe = recipeList.GetComponent<RecipeList>().SelectedRecipe;
        if (selectRecipe == null)
        {
            GetComponent<Button>().interactable = false;
            return;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            makeFree();
        }

        if (recipeMaterial.GetComponent<RecipeMaterial>().canMaking() == 0)
        {
            GetComponent<Button>().interactable = false;
            return;
        }

        GetComponent<Button>().interactable = true;
    }

    public void make()
    {
        GameObject selectRecipe = recipeList.GetComponent<RecipeList>().SelectedRecipe;

        if (!inventory.CheckCanAddItem(selectRecipe.GetComponent<RecipeController>().recipe.Result, 1))
            return;

        if (selectRecipe.GetComponent<RecipeController>().recipe.Material1 != null)
            inventory.subItem(selectRecipe.GetComponent<RecipeController>().recipe.Material1, selectRecipe.GetComponent<RecipeController>().recipe.Material1_num);

        if (selectRecipe.GetComponent<RecipeController>().recipe.Material2 != null)
            inventory.subItem(selectRecipe.GetComponent<RecipeController>().recipe.Material2, selectRecipe.GetComponent<RecipeController>().recipe.Material2_num);

        if (selectRecipe.GetComponent<RecipeController>().recipe.Material3 != null)
            inventory.subItem(selectRecipe.GetComponent<RecipeController>().recipe.Material3, selectRecipe.GetComponent<RecipeController>().recipe.Material3_num);

        inventory.AddItem(selectRecipe.GetComponent<RecipeController>().recipe.Result, 1);
        // recipeMaterial.GetComponent<RecipeMaterial>().SetMaterial(selectRecipe.GetComponent<RecipeController>().recipe);

        GetComponent<Button>().interactable = false;

        making.GetComponent<Making>().UnsetRecipe();
    }

    public void makeFree()
    {
        GameObject selectRecipe = recipeList.GetComponent<RecipeList>().SelectedRecipe;

        if (!inventory.CheckCanAddItem(selectRecipe.GetComponent<RecipeController>().recipe.Result, 1))
            return;
   
        inventory.AddItem(selectRecipe.GetComponent<RecipeController>().recipe.Result, 1);
    }
}
