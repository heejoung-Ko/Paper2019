using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    float clock = 0.5f;

    public GameObject sun;

    public GameObject text;

    // Update is called once per frame
    void Update()
    {
        clock += sun.GetComponent<LightController>().getClockCalc() / 360;

        float day = Mathf.Floor(clock);
        float time = clock - day;
        time *= 2 * 12;
        float hour = Mathf.Floor(time);
        float minit = time - hour;
        minit = minit / 10 * 60;

        text.GetComponent<Text>().text = "Day " + day + "   ";
        if (hour < 10)
            text.GetComponent<Text>().text += "0" + hour + " : ";
        else
            text.GetComponent<Text>().text += hour + " : ";

        text.GetComponent<Text>().text += Mathf.Floor(minit) * 10;

        if (Mathf.Floor(minit) * 10 == 0)
            text.GetComponent<Text>().text += "0";

        //Debug.Log("Day " + day + " | " + hour + "h " + Mathf.Floor(minit) * 10 + "m");
    }

    public float GetClock()
    {
        return clock;
    }

    public void SetClock(float _saveClock)
    {
        clock = _saveClock;
    }
}
