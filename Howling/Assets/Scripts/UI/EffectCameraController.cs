using System.Collections;
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

    Vector3 startPosAtk;
    Howling.PlayerMoveScript playerMoveScript;

    private void Start()
    {
        playerMoveScript = FindObjectOfType<Howling.PlayerMoveScript>();  
    }

    public void EffectCameraOn()
    {
        atkEffectImg.gameObject.SetActive(true);
        FadeIn(0.2f, effectOffTime * 0.1f);
        //effectCamera.gameObject.SetActive(true);
        startPosAtk = effectCamera.transform.localPosition;
        StartCoroutine(EffectShake(shakeAmount, effectOffTime));
        Invoke("EffectCameraOff", effectOffTime);
    }

    public void EffectCameraOff()
    {
        //Debug.Log("effect camera off!");
        atkEffectImg.gameObject.SetActive(false);
        //effectCamera.gameObject.SetActive(false);
    }

    public void DieCameraOn()
    {
        dieEffectImg.gameObject.SetActive(true);
        dieCamera.gameObject.SetActive(true);
        dieCamera.transform.localPosition = Vector3.zero;
        dieCamera.transform.Rotate(Vector3.zero);
        DieEffect(0.2f, dieTime);
    }

    public void DieCameraOff()
    {
        EffectCameraOff();
        dieEffectImg.gameObject.SetActive(false);
        if (dieCamera.gameObject.activeSelf == true)
        {
            dieCamera.transform.localPosition = Vector3.zero;
            dieCamera.transform.Rotate(Vector3.zero);
            dieCamera.gameObject.SetActive(false);
        }
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
        StartCoroutine(CoDieFadeIn(startAlpha, endTime));
        StartCoroutine(DieShake(endTime));
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
    IEnumerator CoDieFadeIn(float startAlpha, float fadeInTime)
    {
        Color tempColor = dieEffectImg.color;
        tempColor.a = startAlpha;
        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeInTime;
            dieEffectImg.color = tempColor;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            yield return null;
        }

        dieEffectImg.color = tempColor;
        Invoke("PlayerRespawn", respawnTime);
    }

    public void PlayerRespawn()
    {
        playerMoveScript.Respawn();
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
            dieCamera.transform.Rotate(0, 0, Mathf.Lerp(0, 1, shakeTime));
            yield return null;
        }
    }
}
