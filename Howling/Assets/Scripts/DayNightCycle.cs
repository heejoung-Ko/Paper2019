using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float minitesInDay = 1f;

    float timer;
    public float percentageOfDay;
    float turnSpeed;

    public float dayNightFogDensity;
    public Gradient dayNightColor; 
    public Gradient dayNightFogColor;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;

        RenderSettings.fogDensity = dayNightFogDensity;
        RenderSettings.fogColor = dayNightFogColor.Evaluate(timer);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
        UpdateLights();

        turnSpeed = 360.0f / (minitesInDay * 60.0f) * Time.deltaTime;
        transform.RotateAround(transform.position, transform.right, turnSpeed);

        //Debug.Log(percentageOfDay);
    }

    bool isNight
    {
        get
        {
            if (percentageOfDay > 0.5f) return true;
            return false;
        }
    }

    void UpdateLights()
    {
        Light l = GetComponent<Light>();

        if (isNight)
        {
            if (l.intensity > 0.0f)
            {
                l.intensity -= 0.005f * Time.deltaTime;
                dayNightFogDensity += 0.1f * Time.deltaTime;
            }
        }
        else
        {
            if (l.intensity < 1.0f)
            {
                l.intensity += 0.005f * Time.deltaTime;
                dayNightFogDensity -= 0.1f * Time.deltaTime;
            }
        }
        l.color = dayNightColor.Evaluate(percentageOfDay);
 //       RenderSettings.ambientLight = dayNightColor.Evaluate(timer);
        RenderSettings.fogDensity = dayNightFogDensity * 15f;
        RenderSettings.fogColor = dayNightFogColor.Evaluate(percentageOfDay);
    }

    void CheckTime()
    {
        timer += Time.deltaTime;
        percentageOfDay = timer / (minitesInDay * 60.0f);
        if(timer > (minitesInDay * 60.0f))
        {
            timer = 0.0f;
        }
    }
}
