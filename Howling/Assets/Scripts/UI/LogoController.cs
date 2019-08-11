using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour
{
    public Texture fadeTexture;

    public GameObject Logo;
    public GameObject Loby;

    public Color startColor;
    public Color endColor;
    public Color currentColor;
    public float duration = 3.0f;

    private bool isEndLogo = false;
   
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
        if(!isEndLogo)
        {
            currentColor = Color.Lerp(startColor, endColor, Time.time / duration);

            if(isEndState)
            {
                isEndLogo = true;
            }
        }

        if(isEndLogo)
        {
            Logo.SetActive(false);
            Loby.SetActive(true);
            currentColor = Color.Lerp(endColor, startColor, Time.time / duration);

        }
    }

    bool isEndState
    {
        get
        {
            if (currentColor == endColor) return true;
            else return false;
        }
    }
}
