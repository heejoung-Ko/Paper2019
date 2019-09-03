using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public static FadeInOut instance;

    //This object should be called 'Fader' and placed over the camera
    [SerializeField] private Image m_Fader;

    //This ensures that we don't mash to change spheres
    // bool changing = false;

    void Awake()
    {
        //Check if we found something
        if (m_Fader == null)
            Debug.LogWarning("No Fader object found on camera.");
    }

    //public void ChangeSphere(Transform nextSphere)
    //{
    //    //Start the fading process
    //    StartCoroutine(FadeCamera(nextSphere));
    //}

    public void OnFadeInOut()
    {
        StartCoroutine(FadeCamera());
    }

    public void OnFadeIn(float _time)
    {
        StartCoroutine(FadeIn(_time, m_Fader.color));
    }

    public void OnFadeOut(float _time)
    {
        StartCoroutine(FadeOut(_time, m_Fader.color));
    }

    //    IEnumerator FadeCamera(Transform nextSphere)
    IEnumerator FadeCamera()
    {
        //Ensure we have a fader object
        if (m_Fader != null)
        {

            //Fade the Quad object in and wait 0.75 seconds
            StartCoroutine(FadeIn(0.75f, m_Fader.color));
            yield return new WaitForSeconds(0.80f);

            ////Change the camera position
            //Camera.main.transform.parent.position = nextSphere.position;
  
            //Fade the Quad object out
            StartCoroutine(FadeOut(0.75f, m_Fader.color));
            yield return new WaitForSeconds(0.75f);
        }
        //else
        //{
        //    //No fader, so just swap the camera position
        //    Camera.main.transform.parent.position = nextSphere.position;
        //}
    }

    IEnumerator FadeOut(float time, Color color)
    {
        //While we are still visible, remove some of the alpha colour
        while (color.a > 0.0f)
        {
            color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime / time));
            yield return null;
        }
    }


    IEnumerator FadeIn(float time, Color color)
    {
        //While we aren't fully visible, add some of the alpha colour
        while (color.a < 1.0f)
        {
            color = new Color(color.r, color.g, color.b, color.a + (Time.deltaTime / time));
            yield return null;
        }
    }
}