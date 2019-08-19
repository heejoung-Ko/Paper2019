using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectCameraController : MonoBehaviour
{
    public Image atkEffectImg;
    public Camera effectCamera;
    public float effectOffTime;
    public float shakeAmount;

    Vector3 startPos;

    public void EffectCameraOn()
    {
        atkEffectImg.gameObject.SetActive(true);
        FadeIn(0.2f, effectOffTime * 0.1f);
        //effectCamera.gameObject.SetActive(true);
        startPos = effectCamera.transform.localPosition;
        StartCoroutine(EffectShake(shakeAmount, effectOffTime));
        Invoke("EffectCameraOff", effectOffTime);
    }

    public void EffectCameraOff()
    {
        Debug.Log("effect camera off!");
        atkEffectImg.gameObject.SetActive(false);
        //effectCamera.gameObject.SetActive(false);
    }

    public void FadeIn(float startAlpha, float fadeOutTime)
    {
        StartCoroutine(CoFadeIn(startAlpha, fadeOutTime));
    }

    public void FadeOut(float fadeOutTime)
    {
        StartCoroutine(CoFadeOut(fadeOutTime));
    }

    // 투명 -> 불투명
    IEnumerator CoFadeIn(float startAlpha, float fadeOutTime)
    {
        Color tempColor = atkEffectImg.color;
        tempColor.a = startAlpha;
        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            atkEffectImg.color = tempColor;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            yield return null;
        }

        atkEffectImg.color = tempColor;
        FadeOut(effectOffTime - fadeOutTime);
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

    public IEnumerator EffectShake(float amount, float endTime)
    {
        float shakeTime = 0;
        while (shakeTime <= endTime)
        {
            effectCamera.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + startPos;

            shakeTime += Time.deltaTime * (endTime * 10 - 0.5f);
            yield return null;
        }
        effectCamera.transform.localPosition = startPos;
    }
}
