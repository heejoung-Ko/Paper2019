using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowlingSceneManager : MonoBehaviour
{
    public GameObject Loby;
    public GameObject Loading;
    public Image progressImage;
    public Text progressText;

    public string sceneName = "HowlingScene";
    private float timer = 0f;

    public static HowlingSceneManager instance;
    private SaveLoadController saveNLoad;
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
    public void ClickStart()
    {
        Loby.SetActive(false);
        Loading.SetActive(true);

        Debug.Log("게임 로딩중 ... ..");
        SceneManager.LoadScene(sceneName);

        StartCoroutine(LoadCoroutine());
    }

    public void ClickLoad()
    {
        Loby.SetActive(false);
        Loading.SetActive(true);

        Debug.Log("게임 로드 중 . .. ..!");
        SceneManager.LoadScene(sceneName);


        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log(operation.progress);

        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {

            //float progress = Mathf.Clamp(operation.progress / .9f);

            progressImage.fillAmount = operation.progress;
            progressText.text = operation.progress * 100f + "%";

            Debug.Log(operation.progress);
            yield return null;
            //if(operation.progress >= 0.9f)
            //{
            //    progressImage.fillAmount = Mathf.Lerp(progressImage.fillAmount, 1f, timer);

            //    //if (progressImage.fillAmount == 1.0f)
            //    //{
            //    //    operation.allowSceneActivation = true;
            //    //}
            //}
            //else
            //{
            //    progressImage.fillAmount = Mathf.Lerp(progressImage.fillAmount, operation.progress, timer);
            //    if(progressImage.fillAmount >= operation.progress)
            //    {
            //        timer = 0f;
            //    }
            //}
        }


        Debug.Log(operation.progress);
        operation.allowSceneActivation = true;

        saveNLoad = FindObjectOfType<SaveLoadController>();
        saveNLoad.LoadData();
        gameObject.SetActive(false);
    }
    public void ClickExit()
    {
        Debug.Log("게임 종료 !");
        Application.Quit();
    }
}
