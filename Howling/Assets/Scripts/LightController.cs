using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    float lerpT = 1;
    float rotation;

    // 게임 내 한 시간에 해당하는 현실 시간(초 단위)
    [SerializeField]float oneHour;

    [SerializeField]
    bool isSunRising;

    float dayFogDensity;         // 낮 시간 동안의 fog 밀도 
    [SerializeField]
    float nightFogDensity;       // 밤 시간 동안의 fog 밀도 
    [SerializeField]
    float currentFogDensity;     // 현재 fog 밀도

    Color dayFogColor;          // 낮 동안의 fog 컬러
    [SerializeField]
    Color nightFogColor;        // 밤 동안의 fog 컬러
    [SerializeField]
    Color currentFogColor;      // 현재 fog 컬러

    // Start is called before the first frame update
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
        dayFogColor = RenderSettings.fogColor;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = Time.deltaTime / oneHour / 24 * 360;

        //Debug.Log(transform.rotation);

        transform.Rotate(Vector3.left, rotation);

        //Debug.Log(transform.eulerAngles.x);
        if (transform.eulerAngles.x <= 270)
            isSunRising = true;
        else
            isSunRising = false;

        if (isSunRising)
            SunRise();
        else
            SunSet();

        RenderSettings.fogColor = currentFogColor;
        RenderSettings.fogDensity = currentFogDensity;
    }

    void SunRise()
    {
        float calc = Time.deltaTime / oneHour / 12;

        currentFogColor = Color.Lerp(nightFogColor, dayFogColor, lerpT += calc);

        currentFogDensity = -Mathf.Lerp(-nightFogDensity, -dayFogDensity, lerpT * 2);

        // Debug.Log(test += Time.deltaTime / oneHour / 12);
    }

    void SunSet()
    {
        float calc = Time.deltaTime / oneHour / 12;

        currentFogColor = Color.Lerp(nightFogColor, dayFogColor, lerpT -= calc);

        currentFogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerpT * 2);

        //Debug.Log(test -= Time.deltaTime / oneHosur / 12);
    }

    public float getClockCalc()
    {
        return rotation;
    }
}
