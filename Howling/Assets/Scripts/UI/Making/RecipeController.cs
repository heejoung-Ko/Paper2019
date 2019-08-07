using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeController : MonoBehaviour
{
    public Recipe recipe;

    public RectTransform rect;
    public Image background;
    public Image icon;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        unselect();

        // EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        // 
        // EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        // entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        // entry_PointerEnter.callback.AddListener((eventdata) => { select(); });
        // eventTrigger.triggers.Add(entry_PointerEnter);
        // 
        // EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
        // entry_PointerExit.eventID = EventTriggerType.PointerExit;
        // entry_PointerExit.callback.AddListener((eventdata) => { unselect(); });
        // eventTrigger.triggers.Add(entry_PointerExit);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setRecipe(Recipe r)
    {
        recipe = r;

        icon.sprite = recipe.Result.ItemImage_64x64;
        text.text = recipe.Result.ItemName;
    }

    public void select()
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, 75);

        transform.parent.GetComponent<RecipeList>().select(gameObject);
    }

    public void unselect()
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
    }
}
