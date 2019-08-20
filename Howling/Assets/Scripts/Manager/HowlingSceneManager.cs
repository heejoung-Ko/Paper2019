using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowlingSceneManager : MonoBehaviour
{
    public GameObject Loby;
    public GameObject Loading;
    public Image progressBar;
    public Text progressText;

    public string sceneName = "HowlingScene";

    public static HowlingSceneManager instance;
    private SaveLoadController SaveLoad;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void SceneReset()
    {
        gameObject.SetActive(true);
        Loby.SetActive(true);
        Loading.SetActive(false);
    }

    public void ClickStart()
    {
        Loby.SetActive(false);
        Loading.SetActive(true);

        Debug.Log("게임 로딩중 ... ..");
        //SceneManager.LoadScene(sceneName);

        StartCoroutine(LoadCoroutine());
    }

    public void ClickLoad()
    {
        Loby.SetActive(false);
        Loading.SetActive(true);

        Debug.Log("게임 로드 중 . .. ..!");
        //SceneManager.LoadScene(sceneName);

        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        float timer = 0.0f;

        while (!asyncOperation.isDone)
        {
            yield return null;

            float progress = Mathf.Clamp01(asyncOperation.progress / .9f);

            timer += Time.deltaTime;

            progressBar.fillAmount = asyncOperation.progress;
            progressText.text = progress * 100f + "%";

            if (asyncOperation.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                    break;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, asyncOperation.progress, timer);
                if (progressBar.fillAmount >= asyncOperation.progress)
                {
                    timer = 0f;
                }
            }

        }

        SaveLoad = FindObjectOfType<SaveLoadController>();
        SaveLoad.LoadData();
        gameObject.SetActive(false);
    }
    public void ClickExit()
    {
        Debug.Log("게임 종료 !");
        Application.Quit();
    }
}
