using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCameraController : MonoBehaviour
{
    public Camera effectCamera;
    public float effectOffTime;
    public float shakeAmount;

    Vector3 startPos;

    public void EffectCameraOn()
    {
        effectCamera.gameObject.SetActive(true);
        startPos = effectCamera.transform.localPosition;
        StartCoroutine(EffectShake(shakeAmount, effectOffTime));
        Invoke("EffectCameraOff", effectOffTime);
    }

    public void EffectCameraOff()
    {
        Debug.Log("effect camera off!");
        effectCamera.gameObject.SetActive(false);
    }

    public IEnumerator EffectShake(float amount, float endTime)
    {
        float shakeTime = 0;
        while (shakeTime <= endTime)
        {
            effectCamera.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + startPos;

            shakeTime += Time.deltaTime * 4.5f;
            yield return null;
        }
        effectCamera.transform.localPosition = startPos;
    }
}
