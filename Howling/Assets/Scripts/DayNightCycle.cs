using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public int day = 0;

    public float minitesInDay = 1f;

    public float percentageOfDay;
    float timer;
    float turnSpeed;

    public float dayNightFogDensity;
    public Gradient dayNightColor; 
    public Gradient dayNightFogColor;

    public StatusController statusController;
    public EnemiesManager enemiesManager;
    public GameObject fireFliesControl;

    [SerializeField] private string dayBgmName;
    [SerializeField] private string nightBgmName;

    bool isCanSetTraceAtNightByDay = true;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlayBGM("bgm");
        timer = 0.0f;
        isCanSetTraceAtNightByDay = true;

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

    [HideInInspector]
    public bool isNight
    {
        get
        {
            if (0.3f < percentageOfDay && percentageOfDay < 0.7f)
            {
                statusController.isNight = true;
                enemiesManager.isNight = true;
                if (isCanSetTraceAtNightByDay)
                {
                    isCanSetTraceAtNightByDay = false;
                    enemiesManager.SetTraceAtNightByDay(day);
                }
                return true;
            }
            statusController.isNight = false;
            enemiesManager.isNight = false;
            return false;
        }
    }

    void UpdateLights()
    {
        Light l = GetComponent<Light>();

        if (isNight)
        {
            //SoundManager.instance.PlayBGMEffect(nightBgmName);

            fireFliesControl.SetActive(true);
                
            if (l.intensity > 0.0f)
            {
                l.intensity -= 0.005f * Time.deltaTime;
                dayNightFogDensity += 0.1f * Time.deltaTime;
            }
        }
        else
        {
            //SoundManager.instance.PlayBGMEffect(dayBgmName);

            fireFliesControl.SetActive(false);
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
        //enemiesManager.SetTraceAtNightByTimer(percentageOfDay);
        if (timer > (minitesInDay * 60.0f))
        {
            timer = 0.0f;
            day++;
            isCanSetTraceAtNightByDay = true;
        }
    }

    public float GetTime()
    {
        return timer;
    }

    public void SetTime(float _saveTime, int _saveDay)
    {
        timer = _saveTime;
        day = _saveDay;
    }

    public void SetTimeSleepInTent()
    {
        //percentageOfDay += 0.3f;
        timer += (minitesInDay * 60.0f) * 0.3f;
    }
}
