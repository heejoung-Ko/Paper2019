using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLight : MonoBehaviour
{
    public Gradient nightDayColor;

    public float maxIntensity;
    public float minIntensity;
    public float minPoint;

    public float maxAmbient;
    public float minAmbient;
    public float minAmbientPoint;

    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale;

    public float dayAtmosphereThickness;
    public float nightAtmosphereThickness;

    public Vector3 dayRotateSpeed;
    public Vector3 nightRotateSpeed;

    public float skySpeed;

    public Transform stars;
    Light mainLight;
    Skybox sky;
    Material skyMat;

    [Header("Monitoring")]
    public float dot;
    
    //public Transform worldProbe;

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;    
    }

    // Update is called once per frame
    void Update()
    {
        float tRange = 1 - minPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

        mainLight.intensity = i;

        tRange = 1 - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
        i = ((maxAmbient - minAmbient) * dot) + minAmbient;
        RenderSettings.ambientIntensity = i;

        mainLight.color = nightDayColor.Evaluate(dot);
        RenderSettings.ambientLight = mainLight.color;

        RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);


        if (dot > 0)
        {
            transform.Rotate(dayRotateSpeed * Time.deltaTime * skySpeed);
        } else
        {
            transform.Rotate(nightRotateSpeed * Time.deltaTime * skySpeed);
        }


        stars.transform.rotation = transform.rotation;
        
        //Vector3 tvec = Camera.main.transform.position;
        //worldProbe.transform.position = tvec;
    }
}
