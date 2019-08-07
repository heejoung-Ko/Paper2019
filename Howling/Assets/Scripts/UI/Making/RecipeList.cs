using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeList : MonoBehaviour
{
    public GameObject Making;

    public GameObject RecipeTool;      // 레시피 툴

    public GameObject SelectedRecipe;

    // Start is called before the first frame update
    void Start()
    {
        Recipe[] recipes = Resources.LoadAll<Recipe>("Recipe");

        for (int i = 0; i < recipes.Length; i++)
        {
            GameObject go = Instantiate(RecipeTool);
            RecipeController recipeController = go.GetComponent<RecipeController>();
            recipeController.setRecipe(recipes[i]);
            go.transform.SetParent(transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void select(GameObject go)
    {
        if (SelectedRecipe != null)
            SelectedRecipe.GetComponent<RecipeController>().unselect();
        SelectedRecipe = go;

        Making.GetComponent<Making>().SetRecipe();
    }

    public void clearSelect()
    {
        if (SelectedRecipe != null)
            SelectedRecipe.GetComponent<RecipeController>().unselect();
        SelectedRecipe = null;
    }
}
