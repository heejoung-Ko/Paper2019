using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour
{
    public Texture fadeTexture;
    public GameObject Logo;
    public GameObject Menu;
    public Color startColor;
    public Color endColor;
    public Color currentColor;
    public float duration = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentColor = startColor;
        //Destroy(fadeTexture, duration + 1);
    }

    private void OnGUI()
    {
        GUI.depth = -10;
        GUI.color = currentColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }

    private void FixedUpdate()
    {
        currentColor = Color.Lerp(startColor, endColor, Time.time / duration);

        if(currentColor == endColor)
        {
            Logo.SetActive(false);
            Invoke("SceneChange", 0.5f);
        }
    }

    void SceneChange()
    {
        Menu.SetActive(true);



    }
}
