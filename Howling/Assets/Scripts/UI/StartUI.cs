using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool isStart;
    public Image[] startImg;

    private bool isOnMouse = false;
    private float fillGauge = 0f;
    private float maxGauge = 10f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isStart)
            {
                //LoadingSceneUI.LoadScene("MainScene");
            }
            else if (!isStart)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        startImg[0].gameObject.SetActive(false);
        startImg[1].gameObject.SetActive(true);
        startImg[2].gameObject.SetActive(true);
        isOnMouse = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        startImg[0].gameObject.SetActive(true);
        startImg[1].gameObject.SetActive(false);
        startImg[2].gameObject.SetActive(false);
        isOnMouse = false;
    }

    void Update()
    {
        if (isOnMouse)
        {
            fillGauge += 0.5f;
            if (fillGauge >= maxGauge) fillGauge = maxGauge;
        }
        else
        {
            fillGauge -= 0.5f;
            if (fillGauge <= 0) fillGauge = 0;
        }
        startImg[2].fillAmount = (float)fillGauge / maxGauge;
    }
}
