using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    float time = 0f;

    public GameObject sun;

    public GameObject text;

    // Update is called once per frame
    void Update()
    {
//        clock += sun.GetComponent<LightController>().getClockCalc() / 360;

        time = sun.GetComponent<DayNightCycle>().percentageOfDay;
        float day = sun.GetComponent<DayNightCycle>().day;

//        float time = clock - day;
        time *= 2 * 12;
        float hour = Mathf.Floor(time);
        float minit = time - hour;
        minit = minit / 10 * 60;

        text.GetComponent<Text>().text = "Day " + day + "   ";
        
        if(hour < 12)
        {
            text.GetComponent<Text>().text += (hour + 12) + " : ";
        }
        else if (hour >= 12)
        {
            hour -= 10;

            if (hour < 10)
                text.GetComponent<Text>().text += "0" + hour + " : ";
            else
                text.GetComponent<Text>().text += hour + " : ";
        }

        text.GetComponent<Text>().text += Mathf.Floor(minit) * 10;

        if (Mathf.Floor(minit) * 10 == 0)
            text.GetComponent<Text>().text += "0";

        //Debug.Log("Day " + day + " | " + hour + "h " + Mathf.Floor(minit) * 10 + "m");
    }
}
