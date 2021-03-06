﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectCameraController : MonoBehaviour
{
    [Header("Attack Effect Setting")]
    public Image atkEffectImg;
    public Camera effectCamera;
    public float effectOffTime;
    public float shakeAmount;

    [Header("Game Over Effect Setting")]
    public Image dieEffectImg;
    public Camera dieCamera;
    public float dieTime;
    public float respawnTime;
    [HideInInspector] public bool isGameOver;

    [Header("Sleep In Tent Effect Setting")]
    public Image sleepEffectImg;
    [HideInInspector] public bool isSleepInTent;
    private float sleepTime = 1f;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private StatusController statusController;

    Vector3 startPosAtk;
    Howling.PlayerMoveScript playerMoveScript;
    UIManagerController uiManagerController;
    public Howling.Inventory inventory;

    private void Start()
    {
        playerMoveScript = FindObjectOfType<Howling.PlayerMoveScript>();
        uiManagerController = FindObjectOfType<UIManagerController>();
        startPosAtk = new Vector3(0, 0, -0.4f);
    }

    public void EffectCameraOn()
    {
        atkEffectImg.gameObject.SetActive(true);
        FadeIn(0.2f, effectOffTime * 0.1f);
        StartCoroutine(EffectShake(shakeAmount, effectOffTime));
        Invoke("EffectCameraOff", effectOffTime);
    }

    public void EffectCameraOff()
    {
        atkEffectImg.gameObject.SetActive(false);
    }

    public void DieCameraOn()
    {
        dieEffectImg.gameObject.SetActive(true);
        effectCamera.gameObject.SetActive(false);
        dieCamera.gameObject.SetActive(true);
        dieCamera.transform.localPosition = new Vector3(0, 0, -0.4f);
        //dieCamera.transform.rotation = Quaternion.Euler(0, 0, 0);

        isGameOver = true;
        DieEffect(0.2f, dieTime);
        inventory.isGameOver = true;
        uiManagerController.isGameOver = true;
        uiManagerController.enterUIEffectCamera();
    }

    public void DieCameraOff()
    {
        EffectCameraOff();
        dieEffectImg.gameObject.SetActive(false);
        sleepEffectImg.gameObject.SetActive(false);
        if (dieCamera.gameObject.activeSelf == true)
        {
            dieCamera.transform.localPosition = new Vector3(0, 0, -0.4f);
            dieCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //dieCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            dieCamera.gameObject.SetActive(false);
        }
        effectCamera.gameObject.SetActive(true);

        isGameOver = false;
        inventory.isGameOver = false;
        uiManagerController.isGameOver = false;
        uiManagerController.exitUIEffectCamera();
    }

    public void SleepCameraOn()
    {
        EffectCameraOff();
        sleepEffectImg.gameObject.SetActive(true);
        dieCamera.gameObject.SetActive(true);
        effectCamera.gameObject.SetActive(false);
        dieCamera.transform.localPosition = new Vector3(0, 0, -0.4f);

        isSleepInTent = true;
        SleepInTentEffect(0.2f, sleepTime);
        inventory.isGameOver = true;
        uiManagerController.isGameOver = true;
        uiManagerController.enterUIEffectCamera();
    }

    public void SleepCameraOff()
    {
        EffectCameraOff();
        dieEffectImg.gameObject.SetActive(false);
        sleepEffectImg.gameObject.SetActive(false);
        if (dieCamera.gameObject.activeSelf == true)
        {
            dieCamera.transform.localPosition = new Vector3(0, 0, -0.4f);
            dieCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //dieCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            dieCamera.gameObject.SetActive(false);
        }
        effectCamera.gameObject.SetActive(true);

        isSleepInTent = false;
        inventory.isGameOver = false;
        uiManagerController.isGameOver = false;
        uiManagerController.exitUIEffectCamera();
    }

    public void FadeIn(float startAlpha, float fadeInTime)
    {
        StartCoroutine(CoFadeIn(startAlpha, fadeInTime));
    }

    public void FadeOut(float fadeOutTime)
    {
        StartCoroutine(CoFadeOut(fadeOutTime));
    }

    public void DieEffect(float startAlpha, float endTime)
    {
        StartCoroutine(CoDieFadeIn(startAlpha, endTime, dieEffectImg));
        StartCoroutine(DieShake(endTime));
    }

    public void SleepInTentEffect(float startAlpha, float endTime)
    {
        StartCoroutine(CoDieFadeIn(startAlpha, endTime, sleepEffectImg));
    }

    // 투명 -> 불투명
    IEnumerator CoFadeIn(float startAlpha, float fadeInTime)
    {
        Color tempColor = atkEffectImg.color;
        tempColor.a = startAlpha;
        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeInTime;
            atkEffectImg.color = tempColor;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            yield return null;
        }

        atkEffectImg.color = tempColor;
        FadeOut(effectOffTime - fadeInTime);
    }

    // 불투명 -> 투명
    IEnumerator CoFadeOut(float fadeOutTime)
    {
        float tempFadeOutTime = fadeOutTime * 0.7f;
        new WaitForSeconds(tempFadeOutTime);
        Color tempColor = atkEffectImg.color;
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / tempFadeOutTime;
            atkEffectImg.color = tempColor;

            if (tempColor.a <= 0f) tempColor.a = 0f;

            yield return null;
        }
        atkEffectImg.color = tempColor;
    }

    // 투명 -> 불투명
    IEnumerator CoDieFadeIn(float startAlpha, float fadeInTime, Image image)
    {
        Color tempColor = image.color;
        tempColor.a = startAlpha;
        //float time = 0f;

        while (tempColor.a < 1f)
        {
            //Debug.Log(time += Time.deltaTime);
            tempColor.a += Time.deltaTime / fadeInTime;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            image.color = tempColor;

            yield return null;
        }

        //Debug.Log(time += Time.deltaTime);

        image.color = tempColor;
        if (isGameOver)
            Invoke("PlayerRespawn", respawnTime);
        else if (isSleepInTent)
            StartCoroutine(CoSleepFadeOut(sleepTime, sleepEffectImg));
    }

    // 불투명 -> 투명
    IEnumerator CoSleepFadeOut(float fadeOutTime, Image image)
    {
        if (isGameOver)
        {
            dieCamera.transform.localPosition = new Vector3(0, 0, -0.4f);
            dieCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (isSleepInTent)
        {
            dayNightCycle.SetTimeSleepInTent();
            statusController.SetStatusSleepInTent();
        }

        Color tempColor = image.color;
        tempColor.a = 1f;
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeOutTime;
            image.color = tempColor;

            if (tempColor.a <= 0f) tempColor.a = 0f;

            yield return null;
        }
        image.color = tempColor;

        if (isGameOver)
            DieCameraOff();
        if (isSleepInTent)
            SleepCameraOff();
    }

    public void PlayerRespawn()
    {
        if (!isGameOver) return;
        playerMoveScript.Respawn();
        StartCoroutine(CoSleepFadeOut(dieTime, dieEffectImg));
    }

    public IEnumerator EffectShake(float amount, float endTime)
    {
        float shakeTime = 0;
        while (shakeTime <= endTime)
        {
            effectCamera.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + startPosAtk;

            shakeTime += Time.deltaTime * (endTime * 9);
            yield return null;
        }
        effectCamera.transform.localPosition = startPosAtk;
    }

    public IEnumerator DieShake(float endTime)
    {
        float shakeTime = 0;
        while (shakeTime <= endTime)
        {
            shakeTime += Time.deltaTime * 1.5f;
            dieCamera.transform.localPosition = new Vector3(0, Mathf.Lerp(0, -1, shakeTime), 0);
            Quaternion rot = dieCamera.transform.rotation;
            dieCamera.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z + Mathf.Lerp(0, 30, shakeTime));

            yield return null;
        }
    }
}
