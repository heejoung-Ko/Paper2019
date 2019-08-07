using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Making : MonoBehaviour
{
    public GameObject RecipeList;
    public GameObject RecipeMaterial;
    public GameObject Explation;

    public void SetRecipe()
    {
        Explation.GetComponent<Text>().text = RecipeList.GetComponent<RecipeList>().SelectedRecipe.GetComponent<RecipeController>().recipe.Explanation;
        RecipeMaterial.GetComponent<RecipeMaterial>().SetMaterial(RecipeList.GetComponent<RecipeList>().SelectedRecipe.GetComponent<RecipeController>().recipe);
    }

    public void UnsetRecipe()
    {
        Explation.GetComponent<Text>().text = "";
        RecipeMaterial.GetComponent<RecipeMaterial>().UnsetMaterial();
        RecipeList.GetComponent<RecipeList>().clearSelect();
    }
}
