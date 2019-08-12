using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneUI : MonoBehaviour
{

    public static SaveLoadController saveNLoad;
    public static string nextScene;
    [SerializeField] Image progressBar;
    static bool isLoad;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    string nextSceneName;
    public static void LoadScene(string sceneName, bool isLoadScene)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("HowlingLoadScene");
        isLoad = isLoadScene;
    }

    IEnumerator LoadScene()
    {
        yield return null;


        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;


        float timer = 0.0f;
        while (!asyncOperation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (asyncOperation.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                    asyncOperation.allowSceneActivation = true;
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


        if (isLoad)
        {
            saveNLoad = FindObjectOfType<SaveLoadController>();
            saveNLoad.LoadData();
        }


    }
}
